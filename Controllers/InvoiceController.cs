using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyHubAPI.Models.DTOs;
using StudyHubAPI.Models.DTOs.Invoice;
using StudyHubAPI.Models.Filter;
using StudyHubAPI.Services;
using StudyHubAPI.Utils;

namespace StudyHubAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly InvoiceService _invoiceService;

        public InvoiceController(InvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        [HttpGet("{Id:int}", Name = "GetInvoice")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<InvoiceDetailsDto>> GetInvoiceById(int Id)
        {
            if (Id <= 0)
            {
                return BadRequest(new { Error = " InvoiceID can't be zero or less" });
            }

            var result = await _invoiceService.GetInvoiceById(Id);

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



        [HttpGet("All", Name = "GetAllInvoices")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResponse<InvoiceSummaryDto>>> GetAllInvoices([FromQuery] InvoiceQueryFilter filter)
        {
            var InvoicesList = await _invoiceService.GetAllInvoices(filter);

            if (InvoicesList.TotalCount == 0)
            {
                return NotFound(new { Error = "No Invoices Found" });
            }

            return Ok(InvoicesList);
        }




    }
}
