using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyHubAPI.Models.DTOs.Country;
using StudyHubAPI.Services;

namespace StudyHubAPI.Controllers
{
    [Authorize]
    [Route("api/Country")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        private readonly CountryService _countryService;

        public CountryController(CountryService countryService)
        {
            _countryService = countryService;
        }


        [HttpGet("All", Name = "GetAllCountries")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<CountrySummaryDto>>> GetAllCountries()
        {

            var result = await _countryService.GetAllCountries();


            if (!result.IsSuccess)
            {
                return NotFound(new { Error =result.ErrorMessage});
            }

            return Ok(result.Data);
        }
    }
}
