using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudyHubAPI.Models.DTOs.LoginInfo;
using StudyHubAPI.Services;
using StudyHubAPI.Utils;

namespace StudyHubAPI.Controllers
{
    [Authorize]
    [Route("api/LoginInfo")]
    [ApiController]
    public class LoginInfoController : ControllerBase
    {
        private readonly LoginInfService _loginInfService;

        public LoginInfoController(LoginInfService loginInfService)
        {
            _loginInfService = loginInfService;
        }


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
                    ResultType.Conflict => Conflict(new { error = result.ErrorMessage }),
                    _  => BadRequest(new { error = result.ErrorMessage ?? "Operation failed." })
                };
            }

            return Ok(result.Data);
        }






    }
}
