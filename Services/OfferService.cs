using StudyHubAPI.Models.DTOs.Offer;
using StudyHubAPI.Models.Entities;
using StudyHubAPI.Repositories;
using StudyHubAPI.Utils;

namespace StudyHubAPI.Services
{
    public class OfferService
    {
        private readonly OfferRepository _offerRepository;

        public OfferService(OfferRepository offerRepository)
        {
            _offerRepository = offerRepository;
        }

        public async Task<ServiceResult<int>> AddOffer(CreateOfferDto newOffer)
        {
            if(newOffer.MaximumDiscountAmount.HasValue && newOffer.MinimumDiscountAmount.HasValue && newOffer.MaximumDiscountAmount < newOffer.MinimumDiscountAmount)
            {
                return ServiceResult<int>.Failure(ResultType.BadRequest, "Maximum discount amount cannot be less than minimum discount amount.");
            }

            if(newOffer.StartDate >= newOffer.EndDate)
            {
                return ServiceResult<int>.Failure(ResultType.BadRequest, "Start date must be before end date.");
            }

            if (newOffer.StartDate < DateTime.UtcNow.AddMinutes(-2))
            {
                return ServiceResult<int>.Failure(ResultType.BadRequest, "Start date cannot be in the past.");
            }
            if(newOffer.EndDate <= DateTime.UtcNow)
            {
                return ServiceResult<int>.Failure(ResultType.BadRequest, "End date cannot be in the past.");
            }
            if (await _offerRepository.IsThereAnActiveOffer(newOffer.StartDate, newOffer.EndDate))
            {
                return ServiceResult<int>.Failure(ResultType.Conflict, "There is already an active offer.");
            }

            return ServiceResult<int>.Success(await _offerRepository.AddOffer(new Offers
            {
                OfferName = newOffer.OfferName,
                OfferPercentage = newOffer.OfferPercentage,
                StartDate = newOffer.StartDate,
                EndDate = newOffer.EndDate,
                MaximumDiscountAmount = newOffer.MaximumDiscountAmount,
                MinimumDiscountAmount = newOffer.MinimumDiscountAmount
            }));
        }

        public async Task<ServiceResult<OfferSummaryDto>> GetOfferById(int offerId)
        {
            var offer = await _offerRepository.GetOfferByIdDto(offerId);
            if (offer == null)
            {
                return ServiceResult<OfferSummaryDto>.Failure(ResultType.NotFound, "Offer not found.");
            }
            return ServiceResult<OfferSummaryDto>.Success(offer);
        }

        public async Task<ServiceResult<List<OfferSummaryDto>>> GetAllOffers()
        {
            var offers = await _offerRepository.GetAllOffers();
            if(offers.Count == 0)
            {
                return ServiceResult<List<OfferSummaryDto>>.Failure(ResultType.NotFound, "No offers found.");
            }   
            return ServiceResult<List<OfferSummaryDto>>.Success(offers);
        }

        public async Task<ServiceResult> UpdateOffer(int offerId, UpdateOfferDto dto)
        {
            var offer = await _offerRepository.GetOfferById(offerId);

            if (offer == null)
            {
                return ServiceResult.Failure(ResultType.NotFound, "Offer not found.");
            }

          
            var effectiveStartDate = dto.StartDate ?? offer.StartDate;
            var effectiveEndDate = dto.EndDate ?? offer.EndDate;


            if (dto.StartDate.HasValue && dto.StartDate.Value < DateTime.UtcNow)
            {
                return ServiceResult.Failure(ResultType.BadRequest, "Start date cannot be in the past.");
            }

            if (dto.EndDate.HasValue && dto.EndDate.Value <= DateTime.UtcNow.AddMinutes(-2))
            {
                return ServiceResult.Failure(ResultType.BadRequest, "End date cannot be in the past.");
            }

            if (effectiveEndDate <= effectiveStartDate)
            {
                return ServiceResult.Failure(ResultType.BadRequest, "End date must be after the start date.");
            }

            if (!string.IsNullOrWhiteSpace(dto.OfferName))
            {
                offer.OfferName = dto.OfferName;
            }

            if (dto.StartDate.HasValue)
            {
                offer.StartDate = dto.StartDate.Value;
            }

            if (dto.EndDate.HasValue)
            {
                offer.EndDate = dto.EndDate.Value;
            }

            await _offerRepository.SaveChange();

            return ServiceResult.Success();
        }


    }
}