using Azure;
using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SalesTracker.Application.DTOs;
using SalesTracker.Application.Interfaces;
using SalesTracker.Shared.Constants;
using SalesTracker.Shared.Responses;
using SalesTracker.Shared.Settings;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly AuthenticationSettings _authSettings;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IUserService userService, IOptions<AuthenticationSettings> options, ILogger<AuthController> logger)
    {
        _userService = userService;
        _authSettings = options.Value;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<ReadUserDto>>> Register(RegisterDto dto)
    {
        _logger.LogInformation("Attempting to register new user: {Username}", dto.Username);

        var user = await _userService.RegisterAsync(dto);
        if (user is null)
        {
            _logger.LogWarning("Registration failed — username already taken: {Username}", dto.Username);
            return BadRequest(ApiResponse<string>.Fail(APIMessages.UsernameTaken));
        }

        _logger.LogInformation("User registered successfully: {Username}", dto.Username);
        return Ok(ApiResponse<ReadUserDto>.Ok(user, APIMessages.RegisterSuccess));
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<TokenResponseDto>>> Login(LoginDto dto)
    {
        _logger.LogInformation("Attempting login for user: {Username}", dto.Username);

        var token = await _userService.LoginAsync(dto);
        if (token is null)
        {
            _logger.LogWarning("Login failed for username: {Username}", dto.Username);
            return BadRequest(ApiResponse<string>.Fail(APIMessages.InvalidCredentials));
        }

        _logger.LogInformation("Login succeeded for user: {Username}", dto.Username);
        SetRefreshTokenCookie(token.RefreshToken);

        return Ok(ApiResponse<TokenResponseDto>.Ok(token, APIMessages.LoginSuccess));
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        _logger.LogInformation("Processing refresh token");

        if (string.IsNullOrEmpty(refreshToken))
        {
            _logger.LogWarning("Refresh token missing from cookies");
            return Unauthorized(ApiResponse<string>.Fail(APIMessages.RefreshTokenMissing));
        }

        var result = await _userService.RefreshTokenAsync(refreshToken);
        if (result is null)
        {
            _logger.LogWarning("Invalid or expired refresh token");
            return Unauthorized(ApiResponse<string>.Fail(APIMessages.ExpiredOrInvalidToken));
        }

        _logger.LogInformation("Refresh token successful — new tokens issued");
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

        _logger.LogInformation("Setting refresh token cookie with expiry of {TTL} days", _authSettings.RefreshTokenTTL);
        Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
    }
}
