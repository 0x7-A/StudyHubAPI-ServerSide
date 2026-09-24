using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyHubAPI.Models.DTOs.Review;
using StudyHubAPI.Services;
using StudyHubAPI.Utils;

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
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ReviewDetailsDto>> GetReviewByID(int reviewID)
        {
            if(reviewID <= 0)
            {
                return BadRequest("ReviewID  can't be zero or less ");
            }

            var result = await _reviewService.GetReviewByID(reviewID);

            if(!result.IsSuccess)
            {
                return result.Type switch
                {
                    ResultType.NotFound => NotFound(new { Error = result.ErrorMessage }),
                    ResultType.Failure => BadRequest(new { Error = result.ErrorMessage ?? "Operation failed." }),
                    _ => BadRequest(new { Error  = result.ErrorMessage ?? "Operation failed." })
                };
            }

            return Ok(result.Data);
                        
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
