using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyHubAPI.Models.DTOs.Damage;
using StudyHubAPI.Utils;
using StudyHubAPI.Services;
using Microsoft.AspNetCore.Http.HttpResults;
namespace StudyHubAPI.Controllers
{


    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DamagesController : ControllerBase
    {
        private readonly DamagesService _damagesService;

        public DamagesController(DamagesService damagesService)
        {
            _damagesService = damagesService;
        }


        [HttpPost("Add", Name = "AddDamage")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<ActionResult> AddDamage(CreateDamageDto dto)
        {
            var result = await _damagesService.AddDamagedRecord(dto);
            if (!result.IsSuccess)
            {
                return result.Type switch
                {
                    ResultType.Failure => BadRequest(new { Error = result.ErrorMessage ?? "Operation failed." }),
                    _ => BadRequest(new { Error = result.ErrorMessage ?? "Operation failed." })
                };
            }

            return CreatedAtRoute("GetDamageRecord", new { paymentID = result.Data }, result.Data);
        }


        [HttpGet("{paymentID}", Name = "GetDamageRecord")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult> GetDamageRecord(int paymentID)
        {
            var result = await _damagesService.GetDamageRecord(paymentID, $"{Request.Scheme}://{Request.Host}");
            if (!result.IsSuccess)
            {
                return result.Type switch
                {
                    ResultType.Failure => BadRequest(new { Error = result.ErrorMessage ?? "Operation failed." }),
                    _ => BadRequest(new { Error = result.ErrorMessage ?? "Operation failed." })
                };
            }

            return Ok(result.Data);
        }



        [AllowAnonymous] // temparily allowing anonymous access for testing purposes
        [HttpGet("{imageId:int}/file")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult> GetImageFile(int PaymentID)
        {
            if (PaymentID <= 0)
            {
                return BadRequest(new { error = "Image ID can't be zero or less" });
            }

            var result = await _damagesService .GetImageFileAsync  (PaymentID);

            if (!result.IsSuccess) 
            {
                return result.Type switch
                {
                    ResultType.BadRequest => BadRequest(new { error = result.ErrorMessage }),
                    ResultType.NotFound => NotFound(new { error = result.ErrorMessage }),
                    ResultType.Failure => BadRequest(new { error = result.ErrorMessage ?? "Operation failed." }),
                    _ => BadRequest(new { Error = result.ErrorMessage ?? "Operation failed." })
                };
            }


            return PhysicalFile(result.Data.FilePath, result.Data.ContentType);
        }


    }
}
