using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyHubAPI.Models.DTOs.LoginInfo;
using StudyHubAPI.Services;
using StudyHubAPI.Utils;

namespace StudyHubAPI.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/LoginInfo")]
    [ApiController]
    public class LoginInfoController : ControllerBase
    {
        private readonly LoginInfoService _loginInfService;

        public LoginInfoController(LoginInfoService loginInfService)
        {
            _loginInfService = loginInfService;
        }

        [Authorize]
        [HttpPost("Add", Name = "AddLoginInfo")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult> AddLoginInfo(CreateLoginInfoDto dto)
        {
            var result = await _loginInfService.AddLoginInfo(dto);


            if (!result.IsSuccess)
            {
                return result.Type switch
                {
                    ResultType.Conflict => Conflict(new { Error = result.ErrorMessage }),
                    _  => BadRequest(new { Error = result.ErrorMessage ?? "Operation failed." })
                };
            }

            return Ok(result.Data);
        }

        [HttpPatch("Update/{PersonID:int}", Name = "UpdateLoginInfoByPersonID")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult> UpdateLoginInfoByPersonID(int PersonID, UpdateLoginInfoDto dto)
        {
            if(PersonID <0)
            {
                return BadRequest(new { Error = "PersonID can't be zero or less " });
            }



            var result = await _loginInfService.UpdateLoginInfo(PersonID, dto);


            if (!result.IsSuccess)
            {
                return result.Type switch
                {
                    ResultType.Conflict => Conflict(new { Error = result.ErrorMessage }),
                    _ => BadRequest(new { Error = result.ErrorMessage ?? "Operation failed." })
                };
            }

            return NoContent();
        }

        [HttpPatch("PromptToAdmin/{PersonID:int}", Name = "PromptToAdmin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> PromptToAdmin(int PersonID)
        {
            if (PersonID < 0)
            {
                return BadRequest(new { Error = "PersonID can't be zero or less " });
            }

            var result = await _loginInfService.PromptToAdmin(PersonID);


            if (!result.IsSuccess)
            {
                return BadRequest(new { error = result.ErrorMessage ?? "Operation failed." });
            }

            return NoContent();
        }


    }
}
