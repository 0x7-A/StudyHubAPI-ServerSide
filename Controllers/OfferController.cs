using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudyHubAPI.Models.DTOs.Admin;
using StudyHubAPI.Models.DTOs.Offer;
using StudyHubAPI.Services;
using StudyHubAPI.Utils;

namespace StudyHubAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OfferController : ControllerBase
    {
        private readonly OfferService _offerService;

        public OfferController(OfferService offerService)
        {
            _offerService = offerService;
        }

        [HttpPost("Add", Name = "AddOffer")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult> AddOffer(CreateOfferDto dto)
        {
            var result = await _offerService.AddOffer(dto);

            if (!result.IsSuccess)
            {
                return result.Type switch
                {

                    ResultType.NotFound => NotFound(new { Error = result.ErrorMessage }),
                    ResultType.Conflict => Conflict(new { Error = result.ErrorMessage }),
                    _ => BadRequest(new { Error = result.ErrorMessage })
                };
            }

            return CreatedAtRoute("GetOffer", new { Id = result.Data }, result.Data);
        }

        [HttpGet("{Id:int}", Name = "GetOffer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<OfferSummaryDto>> GetOffer(int Id)
        {
            if (Id <= 0)
            {
                return BadRequest(new { Error = "Invalid Id" });
            }

            var result = await _offerService.GetOfferById(Id);

            if (!result.IsSuccess)
            {
                return result.Type switch
                { 
                    ResultType.NotFound => NotFound(new { Error = result.ErrorMessage }),
                    _ => BadRequest(new { Error = result.ErrorMessage })
                };
            }

            return Ok(result.Data);
        }



        [HttpGet("GetAll", Name = "GetAllOffers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<IEnumerable<OfferSummaryDto>>> GetAllOffers()
        {
   

            var result = await _offerService.GetAllOffers();

            if (!result.IsSuccess)
            {
                return result.Type switch
                {
                    ResultType.NotFound => NotFound(new { Error = result.ErrorMessage }),
                    _ => BadRequest(new { Error = result.ErrorMessage })
                };
            }

            return Ok(result.Data);
        }


        [HttpPatch("{Id:int}", Name = "UpdateOffer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult> UpdateOffer(int Id, UpdateOfferDto dto)
        {
            if (Id <= 0)
            {
                return BadRequest(new { Error = "Invalid Id" });
            }

            var result = await _offerService.UpdateOffer(Id, dto);

            if (!result.IsSuccess)
            {
                return result.Type switch
                {
                    ResultType.NotFound => NotFound(new { Error = result.ErrorMessage }),
                    _ => BadRequest(new { Error = result.ErrorMessage })
                };
            }

            return NoContent();
        }




    }

}
