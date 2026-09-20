using Microsoft.EntityFrameworkCore;
using StudyHubAPI.Data;
using StudyHubAPI.Models.DTOs.Review;
using StudyHubAPI.Models.Entities;

namespace StudyHubAPI.Repositories
{
    public class ReviewRepository
    {

        private readonly StudyHubDbContext _context;

      
        public ReviewRepository(StudyHubDbContext context)
        {
            _context = context;
        }

        public async Task<List<Reviews>> GetAllReviews()
        {
            return await _context.Reviews.AsNoTracking().ToListAsync();
        }

        public async Task<Reviews?> GetReviewByID(int ReviewID)
        {
            return await _context.Reviews.FindAsync(ReviewID);
        }


        public async Task<List<Reviews>> GetReviewsByReservationID(int ReservationID)
        {
            return await _context.Reviews.Where(r => r.ReservationID == ReservationID).AsNoTracking().ToListAsync();
        }


        public async Task<int> AddReviews(List<Reviews> reviews)
        {
            _context.Reviews.AddRange(reviews);
            return await _context.SaveChangesAsync();

        }

        public async Task<int> AddReview(Reviews review)
        {
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
            return review.ReviewID;
        }

        public async Task<double?> GetAverageRate()
        {
            return await _context.Reviews.AverageAsync(r => (double?) r.Rate);
        }

        public async Task<List<string>> GetAllComments()
        {
          return await _context.Reviews.Where(r => r.Comment != null)
         .Select(r => r.Comment!).ToListAsync();
        }

        public async Task<double?> GetAverageRateByWorkspaceID(int WorkspaceID)
        {
            
            return await _context.Reviews
                .Where(r => r.Reservation.WorkspaceID == WorkspaceID)
                .AverageAsync(r => (double?)r.Rate);
        }


    }
}
