using AutoMapper;
using Microsoft.Extensions.Logging;
using SalesTracker.Application.DTOs;
using SalesTracker.Application.Interfaces;
using SalesTracker.InfraStructure.Interfaces;
using SalesTracker.InfraStructure.Models.Entities;
using SalesTracker.Shared.Constants;
using SalesTracker.Shared.Exceptions;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepo;
    private readonly IRefreshTokenRepository _refreshTokenRepo;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;
    private readonly ILogger<UserService> _logger;

    public UserService(IUserRepository userRepo, IRefreshTokenRepository refreshTokenRepo, ITokenService tokenService, IMapper mapper, ILogger<UserService> logger)
    {
        _userRepo = userRepo;
        _refreshTokenRepo = refreshTokenRepo;
        _tokenService = tokenService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ReadUserDto> RegisterAsync(RegisterDto dto)
    {
        _logger.LogInformation("Checking if username exists: {Username}", dto.Username);
        if (await _userRepo.UsernameExistsAsync(dto.Username))
        {
            _logger.LogWarning(APIMessages.UsernameTaken);
        }

        var user = _mapper.Map<User>(dto);
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        user.IsActive = true;

        var created = await _userRepo.AddAsync(user);
        _logger.LogInformation("User created successfully: {UserId}", created.Id);

        return _mapper.Map<ReadUserDto>(created);
    }

    public async Task<TokenResponseDto?> LoginAsync(LoginDto dto)
    {
        _logger.LogInformation("Attempting login for user: {Username}", dto.Username);

        var user = await _userRepo.GetByUsernameAsync(dto.Username);
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
        {
            _logger.LogWarning("Login failed — invalid credentials for user: {Username}", dto.Username);
            return null;
        }

        var accessToken = _tokenService.CreateAccessToken(user);
        var tokenEntity = _tokenService.CreateRefreshTokenEntity(user.Id);

        await _refreshTokenRepo.AddAsync(tokenEntity);

        _logger.LogInformation("Login successful — token generated for user: {Username}", dto.Username);

        return new TokenResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = tokenEntity.Token
        };
    }

    public async Task<TokenResponseDto?> RefreshTokenAsync(string refreshToken)
    {
        _logger.LogInformation("Validating refresh token");

        var tokenEntity = await _refreshTokenRepo.GetValidTokenAsync(refreshToken);
        if (tokenEntity == null)
        {
            _logger.LogWarning("Refresh token invalid or expired");
            throw new AppException(APIMessages.ExpiredOrInvalidToken);
        }

        var user = await _userRepo.GetByIdAsync(tokenEntity.UserId);
        if (user == null)
        {
            _logger.LogError("User not found for refresh token. ID: {UserId}", tokenEntity.UserId);
            throw new AppException(APIMessages.UnauthorizedAccess);
        }

        var accessToken = _tokenService.CreateAccessToken(user);
        tokenEntity.UsageCount++;

        string refreshTokenToReturn;

        if (tokenEntity.UsageCount >= tokenEntity.MaxUsageCount)
        {
            _logger.LogInformation("Rotating refresh token for user ID {UserId}", user.Id);
            tokenEntity.IsRevoked = true;

            var newTokenEntity = _tokenService.CreateRefreshTokenEntity(user.Id);
            await _refreshTokenRepo.AddAsync(newTokenEntity);
            refreshTokenToReturn = newTokenEntity.Token;
        }
        else
        {
            _logger.LogInformation("Reusing existing refresh token for user ID {UserId}", user.Id);
            refreshTokenToReturn = tokenEntity.Token;
        }

        await _refreshTokenRepo.UpdateAsync(tokenEntity);
        _logger.LogInformation("Refresh token update complete for user ID {UserId}", user.Id);

        return new TokenResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshTokenToReturn
        };
    }
}
