using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using StudyHubAPI.Models.DTOs.Auth;
using StudyHubAPI.Services;
using System.Security.Cryptography;

namespace StudyHubAPI.Controllers
{
    [Route("api/Auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _AuthService;

        public AuthController(AuthService authService)
        {
            _AuthService = authService;
        }


        [EnableRateLimiting("AuthLimiter")]
        [HttpPost("login", Name = "login")]
        [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> Login([FromBody] LoginRequestDto request)
        {
            var loginResponse = await _AuthService.Login(request);

            if (loginResponse == null)
            {
                return Unauthorized("Invalid credentials");
            }

            return Ok(loginResponse);
        }

        [EnableRateLimiting("AuthLimiter")]
        [AllowAnonymous]
        [HttpPost("refresh")]
        [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<TokenResponse>> Refresh([FromBody] RefreshRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                return BadRequest("Refresh token is required.");
            }

            var tokenResponse = await _AuthService.RefreshTokenAsync(request);

            if (tokenResponse == null)
            {
                return Unauthorized("Invalid, expired, or revoked refresh token.");
            }

            return Ok(tokenResponse);
        }
        [EnableRateLimiting("AuthLimiter")]
        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Logout([FromBody] LogoutRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                return BadRequest("Refresh token is required.");
            }

            var result = await _AuthService.LogoutAsync(request);
            if (!result)
            {
                return BadRequest("Invalid or already revoked token.");
            }

            return Ok(new { message = "Logged out successfully." });
        }

    }
}
