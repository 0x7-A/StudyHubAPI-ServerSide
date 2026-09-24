using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyHubAPI.Models.DTOs;
using StudyHubAPI.Models.DTOs.Customer;
using StudyHubAPI.Models.Enums;
using StudyHubAPI.Models.Filter;
using StudyHubAPI.Services;
using StudyHubAPI.Utils;
using System.Security.Claims;

namespace StudyHubAPI.Controllers
{
    [Authorize]
    [Route("api/Customer")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        CustomerService _customerService;

        public CustomerController(CustomerService customerService)
        {
            _customerService = customerService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("Add", Name = "AddCustomer")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult> AddCustomer(CreateCustomerDto dto)
        {
            var result = await _customerService.AddCustomer(dto);


            if (!result.IsSuccess)
            {
                return result.Type switch
                {
                    ResultType.BadRequest => BadRequest(new { error = result.ErrorMessage }),
                    ResultType.NotFound => NotFound(new { error = result.ErrorMessage }),
                    ResultType.Conflict => Conflict(new { error = result.ErrorMessage }),
                    _ => BadRequest(new { error = result.ErrorMessage ?? "Operation failed." })
                };
            }

            return CreatedAtRoute("GetCustomer", new { Id = result.Data }, result.Data);
        }



        [HttpGet("{Id:int}", Name = "GetCustomer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]


        public async Task<ActionResult<CustomerDetailsDto>> GetCustomer(int Id)
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

            if (User.IsInRole(nameof(PersonRole.Customer)) && currentUserId != Id)
            {
                return Forbid();
            }

            var result = await _customerService.GetCustomerByID(Id);

            if (!result.IsSuccess)
            {
                return result.Type switch
                {
                    ResultType.BadRequest => BadRequest(new { error = result.ErrorMessage }),
                    ResultType.NotFound => NotFound(new { error = result.ErrorMessage }),
                    ResultType.Conflict => Conflict(new { error = result.ErrorMessage }),
                   _ => BadRequest(new { error = result.ErrorMessage ?? "Operation failed." })
                };
            }

            return Ok(result.Data);
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("All", Name = "GetAllCustomers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResponse<CustomerSummaryDto>>> GetAllCustomers([FromQuery] CustomerQueryFilter filter)
        {
            if (filter.pageNumber <= 0 || filter.pageSize <= 0)
            {
                return BadRequest(new { Error = "Page number and page size must be greater than zero." });
            }


            var customersList = await _customerService.GetAllCustomers(filter);


            if (customersList.TotalCount == 0)
            {
                return NotFound(new { Error = "No Customers Found" });
            }

            return Ok(customersList);
        }



        [HttpPatch("{Id:int}", Name = "UpdateCustomer")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult> UpdateCustomer(int Id, UpdateCustomerDto dto)
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

            if (User.IsInRole(nameof(PersonRole.Customer)) && currentUserId != Id)
            {
                return Forbid();
            }

            var result = await  _customerService.UpdateCustomer(Id, dto);


            if (!result.IsSuccess)
            {
                return result.Type switch
                {
                    ResultType.BadRequest => BadRequest(new { error = result.ErrorMessage }),
                    ResultType.NotFound => NotFound(new { error = result.ErrorMessage }),
                    ResultType.Conflict => Conflict(new { error = result.ErrorMessage }),
                    _ => BadRequest(new { error = result.ErrorMessage ?? "Operation failed." })
                };
            }

            return NoContent();
        }


        [Authorize(Roles = "Admin")]
        [HttpDelete("{Id:int}", Name = "DeleteCustomer")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> DeleteCustomer(int Id)
        {
            if (Id <= 0)
            {
                return BadRequest(new { Error = "personID can't be zero or less" });
            }

            var Succseeded = await _customerService.DeleteCustomerByID(Id);

            if (Succseeded)
            {
                return NoContent();
            }

            return NotFound();
        }


    }
}
