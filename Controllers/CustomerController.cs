using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudyHubAPI.Models.DTOs;
using StudyHubAPI.Models.DTOs.Customer;
using StudyHubAPI.Models.Enums;
using StudyHubAPI.Services;
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
        public async Task<ActionResult> AddCustomer(CreateCustomerDto dto)
        {
            int NewPersonID = await _customerService.AddCustomer(dto);


            if (NewPersonID == -1)
            {
                return BadRequest();
            }

            return CreatedAtRoute("GetCustomer", new { Id = NewPersonID }, NewPersonID);
        }



        [HttpGet("{Id:int}", Name = "GetCustomer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CustomerDetailsDto>> GetCustomer(int Id)
        {
            if (Id <= 0)
            {
                return BadRequest("personID can't be zero or less");
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

            var customer = await _customerService.GetCustomerByID(Id);

            if (customer == null)
            {
                return NotFound("The Customer Was not found");
            }


            return Ok(customer);
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("All/{pageNumber:int}/{pageSize:int}", Name = "GetAllCustomers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResponse<CustomerSummaryDto>>> GetAllCustomers(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return BadRequest("Page number and page size must be greater than zero.");
            }


            var customersList = await _customerService.GetAllCustomers(pageNumber, pageSize);


            if (customersList.TotalCount == 0)
            {
                return NotFound("No Customers Found");
            }

            return Ok(customersList);
        }



        [HttpPatch("{Id:int}", Name = "UpdateCustomer")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdateCustomer(int Id, UpdateCustomerDto dto)
        {
            if (Id <= 0)
            {
                return BadRequest("personID can't be zero or less");
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


            var Succseeded = await  _customerService.UpdateCustomer(Id, dto);


            if (Succseeded)
            {
                return NoContent();
            }

            return NotFound();
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
                return BadRequest("personID can't be zero or less");
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
