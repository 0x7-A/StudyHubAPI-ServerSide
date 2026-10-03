using Microsoft.EntityFrameworkCore;
using StudyHubAPI.Data;
using StudyHubAPI.Models.DTOs.Offer;
using StudyHubAPI.Models.Entities;

namespace StudyHubAPI.Repositories
{
    public class OfferRepository
    {
        private readonly StudyHubDbContext _context;

        public OfferRepository(StudyHubDbContext context)
        {
            _context = context;
        }

        public async Task<int> AddOffer(Offers newOffer)
        {
            _context.Offers.Add(newOffer);
            await _context.SaveChangesAsync();
            return newOffer.OfferID;
        }

        public async Task<Offers?> GetOfferById(int offerId)
        {
            return await _context.Offers.FindAsync(offerId);
        }

        public async Task<OfferSummaryDto?> GetOfferByIdDto(int offerId)
        {
            return await _context.Offers.Select(o => new OfferSummaryDto
            {
                OfferID = o.OfferID,
                OfferName = o.OfferName,
                OfferPercentage = o.OfferPercentage,
                StartDate = o.StartDate,
                EndDate = o.EndDate,
                MaximumDiscountAmount = o.MaximumDiscountAmount,
                MinimumDiscountAmount = o.MinimumDiscountAmount
            }).AsNoTracking().FirstOrDefaultAsync(o => o.OfferID == offerId);
        }

        public async Task<List<OfferSummaryDto>> GetAllOffers()
        {
            return await _context.Offers.Select(o => new OfferSummaryDto
            {
                OfferID = o.OfferID,
                OfferName = o.OfferName,
                OfferPercentage = o.OfferPercentage,
                StartDate = o.StartDate,
                EndDate = o.EndDate,
                MaximumDiscountAmount = o.MaximumDiscountAmount,
                MinimumDiscountAmount = o.MinimumDiscountAmount
            }).AsNoTracking().ToListAsync();
        }

        public async Task<int> SaveChange()
        {
            return await _context.SaveChangesAsync();
        }


        public async Task<bool> IsThereAnActiveOffer(DateTime start, DateTime end)
        {
            return await _context.Offers.AnyAsync(o => o.StartDate <= end && o.EndDate >= start);
        }

        public async Task<OfferSummaryDto?> GetActiveOffer()
        {
            var now = DateTime.UtcNow;

            return await _context.Offers.Where(o => o.StartDate <= now && o.EndDate >= now).Select(o => new OfferSummaryDto
            {
                OfferID = o.OfferID,
                OfferPercentage = o.OfferPercentage,
                MaximumDiscountAmount = o.MaximumDiscountAmount,
                MinimumDiscountAmount = o.MinimumDiscountAmount
            }).AsNoTracking().FirstOrDefaultAsync();
        }

    }
}
