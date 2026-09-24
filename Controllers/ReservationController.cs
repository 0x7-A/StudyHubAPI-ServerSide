using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudyHubAPI.Models.DTOs;
using StudyHubAPI.Models.DTOs.Reservation;
using StudyHubAPI.Models.Filter;
using StudyHubAPI.Services;
using StudyHubAPI.Utils;
using System.Security.Claims;

namespace StudyHubAPI.Controllers
{
    [Authorize]
    [Route("api/Reservation")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        ReservationService _reservationService;

        public ReservationController(ReservationService reservationService)
        {
            _reservationService = reservationService;
        }


        [HttpPost("Add", Name = "AddReservation")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]


        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<int>> AddReservation(CreateReservationDto dto)
        {

            var result = await _reservationService.AddReservation(dto);

            if (!result.IsSuccess)
            {
                return result.Type switch
                {
                    ResultType.NotFound => NotFound(new { error = result.ErrorMessage }),
                    ResultType.Failure => BadRequest(new { error = result.ErrorMessage ?? "Operation failed." })
                    _ => BadRequest(new { error = result.ErrorMessage ?? "Operation failed." })
                };
            }

            return CreatedAtRoute("GetReservationByID", new { reservationID = NewReservationID }, NewReservationID);

        }

        [Authorize(Roles = "Admin")]
        [HttpGet("All", Name = "GetAllReservation")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<ActionResult<PagedResponse<ReservationSummaryDto>>> GetAllReservation([FromQuery] ReservationQueryFilter filter)
        {
            if (filter.pageNumber <= 0 || filter.pageSize <= 0)
            {
                return BadRequest("page number and page size can't be zero or less");
            }


            var ReservationList = await _reservationService.GetAllReservation(filter);

            if (ReservationList.TotalCount == 0)
            {
                return NotFound("No Reservation Was Found");
            }

            return Ok(ReservationList); 
        }


        [HttpGet("{reservationID:int}", Name = "GetReservationByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<ActionResult<ReservationDetails>> GetReservationByID(int reservationID)
        {
            if (reservationID <= 0 )
            {
                return BadRequest("ReservationID can't be zero or less");
            }


            var Reservation = await _reservationService.GetReservationByID(reservationID);

            if (Reservation == null)
            {
                return NotFound("No Reservation Was Found");
            }

            return Ok(Reservation);
        }


        [Authorize(Roles = "Admin")]
        [HttpPatch("checkIn/{reservationID:int}", Name = "checkIn")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> CheckIn(int reservationID)
        {
            if (reservationID <= 0)
            {
                return BadRequest("ReservationID can't be zero or less");
            }

            var successed = await _reservationService.CheckIn(reservationID);

            if(successed)
            {
                return NoContent();
            }

            return BadRequest();
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("checkOut/{reservationID:int}", Name = "checkOut")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> CheckOut(int reservationID)
        {
            if (reservationID <= 0)
            {
                return BadRequest("ReservationID can't be zero or less");
            }

            var adminIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(adminIdClaim) || !int.TryParse(adminIdClaim, out int currentAdminId))
            {
                return Unauthorized("Admin identity is missing or invalid.");
            }

            var successed = await _reservationService.CheckOut(reservationID, currentAdminId);

            if (successed)
            {
                return NoContent();
            }

            return BadRequest();
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("cancle/{reservationID:int}", Name = "cancle")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Cancle(int reservationID)
        {
            if (reservationID <= 0)
            {
                return BadRequest("ReservationID can't be zero or less");
            }

            var successed = await _reservationService.Cancle(reservationID);

            if (successed)
            {
                return NoContent();
            }

            return BadRequest();
        }


    }
}
