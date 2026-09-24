using StudyHubAPI.Repositories;
using StudyHubAPI.Models.Entities;
using StudyHubAPI.Models.DTOs.Review;
using StudyHubAPI.Utils;

namespace StudyHubAPI.Services
{
    public class ReviewService
    {
        private readonly ReviewRepository _ReviewRepository;
        private readonly ReservationRepository _ReservationRepository;

        public ReviewService(ReviewRepository ReviewRepository, ReservationRepository reservationRepository)
        {
            _ReviewRepository = ReviewRepository;
            _ReservationRepository = reservationRepository;
        }

        public async Task<ServiceResult<ReviewDetailsDto?>> GetReviewByID(int ReviewID)
        {
            var review = await _ReviewRepository.GetReviewByID(ReviewID);
            if (review == null)
            {
                return ServiceResult<ReviewDetailsDto?>.Failure(ResultType.NotFound, "Review not found");
            }

            return ServiceResult<ReviewDetailsDto?>.Success(new ReviewDetailsDto
            {
                ReviewID = review.ReviewID,
                ReservationID = review.ReservationID,
                Rate = review.Rate,
                Comment = review.Comment
            }, ResultType.Ok);
        }


        public async Task<ServiceResult<int>> AddReview(CreateReviewDto dto)
        {
            // check if this reservation vaild and status = completed
            if(await _ReservationRepository.IsValidReservationForReview(dto.ReservationID)!)
            {
                return ServiceResult<int>.Failure(ResultType.BadRequest, "Review Can done ater the reservation is completed");
            }

            return ServiceResult<int>.Success(await _ReviewRepository.AddReview(new Reviews 
            { ReservationID = dto.ReservationID, Comment = dto.Comment, Rate = dto.Rate }), ResultType.Created);
        }

        public Task<List<string>> GetAllComments()
        {
            return _ReviewRepository.GetAllComments();
        }

        public Task<double?> GetAverageRate()
        {
            return _ReviewRepository.GetAverageRate();
        }


        public Task<double?> GetAverageRateByWorkspaceID(int workspaceID)
        {
            return _ReviewRepository.GetAverageRateByWorkspaceID(workspaceID);
        }

    }
}
