using Microsoft.AspNetCore.Mvc;
using StudyHubAPI.Models.DTOs.Payment;
using StudyHubAPI.Models.Enums;
using StudyHubAPI.Services;
using System.Security.Claims;

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
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult> AddPayment(CreatePaymentDto dto)
        {
            int newPaymentID = await _paymentService.AddPayment(dto);

            if (newPaymentID == -1)
            {
                return BadRequest();
            }

            return CreatedAtRoute("GetPayment", new { Id = newPaymentID }, newPaymentID);
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
                return BadRequest("PaymentID can't be zero or less");
            }

           
            var payment = await _paymentService.GetPaymentByID(Id);
            if (payment == null)
            {
                return NotFound($"No payment found with ID {Id}.");
            }
            var currentUserIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(currentUserIdClaim, out int currentUserId))
            {
                return Unauthorized();
            }

            var IsAdmin = User.IsInRole(nameof(PersonRole.SuperAdmin)) || User.IsInRole(nameof(PersonRole.Admin));

            if (!IsAdmin && currentUserId != payment.CustomerID)
            {
                return Forbid();
            }

            return Ok(payment);
        }


        [HttpPatch("{Id:int}", Name = "UpdatePayment")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> UpdatePayment(int Id, UpdatePaymentDto dto)
        {
            if (Id <= 0)
            {
                return BadRequest("PaymentID can't be zero or less");
            }

             var Successd = await _paymentService.UpdatePayment(Id, dto);

            if(!Successd)
            {
                return NotFound();
            }

            return NoContent();
        }


        [HttpGet("GetAllPendingPayment{Id:int}", Name = "GetAllPendingPayment")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IEnumerable<PaymentDetialsDto>>> GetAllPendingPayment(int CustomerID)
        {
            if (CustomerID <= 0)
            {
                return BadRequest("PaymentID can't be zero or less");
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
