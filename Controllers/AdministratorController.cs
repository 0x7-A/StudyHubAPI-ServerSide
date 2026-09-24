using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyHubAPI.Models.DTOs;
using StudyHubAPI.Models.DTOs.Admin;
using StudyHubAPI.Models.Enums;
using StudyHubAPI.Models.Filter;
using StudyHubAPI.Services;
using StudyHubAPI.Utils;
using System.Security.Claims;

namespace StudyHubAPI.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    [Route("api/Administrator")]
    [ApiController]
    public class AdministratorController : ControllerBase
    {
        private readonly AdministratorService _administratorService;

        public AdministratorController(AdministratorService administratorService)
        {
            _administratorService = administratorService;
        }

       
        [HttpPost("Add", Name = "AddAdministrator")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult> AddAdmin(CreateAdminDto dto)
        {
            var result = await _administratorService.AddAdmin(dto);

            if (!result.IsSuccess)
            {
                return result.Type switch
                {

                    ResultType.NotFound => NotFound(new { error = result.ErrorMessage }),
                    ResultType.Conflict => Conflict(new { error = result.ErrorMessage }),
                    _ => BadRequest(new { error = result.ErrorMessage })
                };
            }

            return CreatedAtRoute("GetAdmin", new { Id = result.Data }, result.Data);
        }


        [HttpGet("{Id:int}", Name = "GetAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AdminDetailsDto>> GetAdminByID(int Id)
        {
            if (Id <= 0 )
            {
                return BadRequest(new { Error = "personID can't be zero or less" });
            }

            var currentUserIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(currentUserIdClaim, out int currentUserId))
            {
                return Unauthorized();
            }

            if (!User.IsInRole(nameof(PersonRole.SuperAdmin)) && currentUserId != Id)
            {
                return Forbid();
            }

            var result = await _administratorService.GetAdminByID(Id);


            if (!result.IsSuccess)
            {
                return result.Type switch
                {
                    ResultType.NotFound => NotFound(new { Error = result.ErrorMessage }),
                    ResultType.Failure => BadRequest(new { Error = result.ErrorMessage ?? "Operation failed." }),
                    _ => BadRequest(new { Error = result.ErrorMessage })

                };
            }

            return Ok(result.Data);
        }



        [HttpGet("All", Name = "GetAllAdministrators")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async  Task<ActionResult<PagedResponse<AdminSummaryDto>>> GetAllAdministrators([FromQuery] AdminQueryFilter filter)
        {
            if (filter.pageNumber <= 0 || filter.pageSize <= 0)
            {
                return BadRequest(new { Error = "Page number and page size must be greater than zero." });
            }


            var AdminsList = await _administratorService.GetAllAdmins(filter);


            if (AdminsList.TotalCount == 0)
            {
                return NotFound(new { Error = "No Admins Found" });
            }

            return Ok(AdminsList);
        }


        [HttpPatch("{Id:int}", Name = "UpdateAdmin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult> UpdateAdmin(int Id, UpdateAdminDto dto)
        {
            if (Id <= 0)
            {
                return BadRequest(new { Error = "personID can't be zero or less" });
            }


            var currentUserIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(currentUserIdClaim, out int currentUserId))
            {
                return Unauthorized();
            }

            if (!User.IsInRole(nameof(PersonRole.SuperAdmin)) && currentUserId != Id)
            {
                return Forbid();
            }

            var result = await _administratorService.UpdateAdmin(Id, dto);


            if (!result.IsSuccess)
            {
                return result.Type switch
                {
                    ResultType.BadRequest => BadRequest(new { Error = result.ErrorMessage }),
                    ResultType.NotFound => NotFound(new { Error = result.ErrorMessage }),
                    ResultType.Conflict => Conflict(new { Error = result.ErrorMessage }),
                    ResultType.Failure => BadRequest(new { Error = result.ErrorMessage ?? "Operation failed." }),
                     _=> BadRequest(new { Error  = result.ErrorMessage })
                };
            }

            return NoContent();
        }



        [HttpDelete("{Id:int}", Name = "DeleteAdmin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> DeleteAdmin(int Id)
        {
            if (Id <= 0)
            {
                return BadRequest(new { Error = "personID can't be zero or less" });
            }

            var Succseeded = await _administratorService.DeleteAdminByID(Id);


            if (Succseeded)
            {
                return NoContent();
            }

            return NotFound();
        }



    }
}
