using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SalesTracker.Application.DTOs;
using SalesTracker.Application.Interfaces;
using SalesTracker.Application.Services;
using SalesTracker.InfraStructure.Models.Entities;
using SalesTracker.Shared.Constants;
using SalesTracker.Shared.Responses;
using SalesTracker.Shared.Settings;

namespace SalesTrackerAPI.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly AuthenticationSettings _authSettings;

        public AuthController(IUserService userService, IOptions<AuthenticationSettings> options)
        {
            _userService = userService;
            _authSettings = options.Value;
        }


        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<ReadUserDto>>> Register(RegisterDto dto)
        {
            var user = await _userService.RegisterAsync(dto);

            if (user is null)
                return BadRequest(ApiResponse<string>.Fail(APIMessages.UsernameTaken));

            return Ok(ApiResponse<ReadUserDto>.Ok(user, APIMessages.RegisterSuccess));
        }


        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<TokenResponseDto>>> Login(LoginDto dto)
        {
            var token = await _userService.LoginAsync(dto);

            if (token is null)
                return BadRequest(ApiResponse<string>.Fail(APIMessages.InvalidCredentials));

            SetRefreshTokenCookie(token.RefreshToken);

            return Ok(ApiResponse<TokenResponseDto>.Ok(token, APIMessages.LoginSuccess));
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized(ApiResponse<string>.Fail(APIMessages.RefreshTokenMissing));

            var result = await _userService.RefreshTokenAsync(refreshToken);

            if (result is null)
                return Unauthorized(ApiResponse<string>.Fail(APIMessages.ExpiredOrInvalidToken));

            SetRefreshTokenCookie(result.RefreshToken);

            return Ok(ApiResponse<TokenResponseDto>.Ok(result, APIMessages.TokenRefreshed));
        }

        private void SetRefreshTokenCookie(string refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true, 
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(_authSettings.RefreshTokenTTL)
            };

            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }
    }

}

