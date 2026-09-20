using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudyHubAPI.Models.DTOs.Review;
using StudyHubAPI.Services;

namespace StudyHubAPI.Controllers
{
    [Authorize]
    [Route("api/Review")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        ReviewService _reviewService;

        public ReviewController(ReviewService reviewService)
        {
            _reviewService = reviewService;
        }


        [HttpPost("Add", Name = "AddReview")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<int>> AddReview(CreateReviewDto dto)
        {
            var NewReviewID = await _reviewService.AddReview(dto);

            return CreatedAtRoute("GetReviewByID", new { id = NewReviewID }, NewReviewID);
        }


        [HttpGet("{reviewID:int}", Name = "GetReviewByID")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ReviewDetailsDto>> GetReviewByID(int reviewID)
        {
            if(reviewID <= 0)
            {
                return BadRequest("ReviewID  can't be zero or less ");
            }

            var Review = _reviewService.GetReviewByID(reviewID);

            if(Review == null)
            {
                return NotFound("Was not Found");
            }

            return Ok(Review);
                        
        }



        [AllowAnonymous]
        [HttpGet("Average", Name = "GetAverageRate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<double?>> GetAverageRate()
        {
            var averageRate = await _reviewService.GetAverageRate();

            return Ok(averageRate);
        }



        [HttpGet("Workspace/{workspaceID:int}", Name = "GetAverageRateByWorkspaceID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<double?>> GetAverageRateByWorkspaceID(int workspaceID)
        {
            if (workspaceID <= 0)
            {
                return BadRequest("WorkspaceID Can't be zero or less");
            }

            
            var averageRate = await _reviewService.GetAverageRateByWorkspaceID(workspaceID);

            return Ok(averageRate);
        }



    }
}
