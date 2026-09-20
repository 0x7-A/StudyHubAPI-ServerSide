using StudyHubAPI.Repositories;
using StudyHubAPI.Models.Entities;
using StudyHubAPI.Models.DTOs.Review;

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

        public async Task<ReviewDetailsDto?> GetReviewByID(int ReviewID)
        {
            var review = await _ReviewRepository.GetReviewByID(ReviewID);
            if (review == null)
            {
                return null;
            }

            return new ReviewDetailsDto
            {
                ReviewID = review.ReviewID,
                ReservationID = review.ReservationID,
                Rate = review.Rate,
                Comment = review.Comment
            };
        }


        public async Task<int> AddReview(CreateReviewDto dto)
        {

            // check if this reservation vaild and status = completed

            if(await _ReservationRepository.IsValidReservationForReview(dto.ReservationID)!)
            {
                return -1;
            }

            var Review = new Reviews { ReservationID = dto.ReservationID, Comment = dto.Comment, Rate = dto.Rate };

            return await _ReviewRepository.AddReview(Review);
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
