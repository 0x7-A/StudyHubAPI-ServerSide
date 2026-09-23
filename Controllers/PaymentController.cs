using Microsoft.AspNetCore.Mvc;
using StudyHubAPI.Models.DTOs.Payment;
using StudyHubAPI.Models.Enums;
using StudyHubAPI.Services;
using StudyHubAPI.Utils;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace StudyHubAPI.Controllers
{
    [Route("api/Payment")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly PaymentService _paymentService;
        public PaymentController(PaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("Add", Name = "AddPayment")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> AddPayment(CreatePaymentDto dto)
        {
            var result = await _paymentService.AddPayment(dto);

            if (!result.IsSuccess)
            {
                return result.Type switch
                {
                    ResultType.NotFound => NotFound(new { error = result.ErrorMessage }),
                    ResultType.Failure => BadRequest(new { error = result.ErrorMessage ?? "Operation failed." })
                };
            }

            return CreatedAtRoute("GetPayment", new { Id = result.Data }, result.Data);
        }


        [HttpGet("{Id:int}", Name = "GetPayment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<PaymentDetialsDto>> GetPaymentByID(int Id)
        {
            if (Id <= 0)
            {
                return BadRequest(new { Error = "PaymentID can't be zero or less" });
            }

            var result = await _paymentService.GetPaymentByID(Id);
            if (!result.IsSuccess)
            {
                return result.Type switch
                {
                    ResultType.BadRequest => BadRequest(new { error = result.ErrorMessage }),
                    ResultType.NotFound => NotFound(new { error = result.ErrorMessage }),
                    ResultType.Failure => BadRequest(new { error = result.ErrorMessage ?? "Operation failed." })
                };
            }



            var currentUserIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(currentUserIdClaim, out int currentUserId))
            {
                return Unauthorized();
            }

            var IsAdmin = User.IsInRole(nameof(PersonRole.SuperAdmin)) || User.IsInRole(nameof(PersonRole.Admin));

            if (!IsAdmin && currentUserId != result.Data.CustomerID)
            {
                return Forbid();
            }

            return Ok(result.Data);
        }


        [HttpPatch("{Id:int}", Name = "UpdatePayment")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdatePayment(int Id, UpdatePaymentDto dto)
        {
            if (Id <= 0)
            {
                return BadRequest(new { Error = "PaymentID can't be zero or less" });
            }

             var result = await _paymentService.UpdatePayment(Id, dto);

            if (!result.IsSuccess)
            {
                return result.Type switch
                {
                    ResultType.BadRequest => BadRequest(new { error = result.ErrorMessage }),
                    ResultType.NotFound => NotFound(new { error = result.ErrorMessage }),
                    ResultType.Failure => BadRequest(new { error = result.ErrorMessage ?? "Operation failed." })
                };
            }

            return NoContent();
        }


        [HttpGet("GetAllPendingPayment{Id:int}", Name = "GetAllPendingPayment")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<PaymentDetialsDto>>> GetAllPendingPayment(int CustomerID)
        {
            if (CustomerID <= 0)
            {
                return BadRequest(new { Error = "PaymentID can't be zero or less" });
            }


            var PaymentList = await _paymentService.GetAllPendingPaymentByCustomerID(CustomerID);

            if (PaymentList == null)
            {
                return NotFound();
            }

            return Ok(PaymentList);
        }



    }
}
