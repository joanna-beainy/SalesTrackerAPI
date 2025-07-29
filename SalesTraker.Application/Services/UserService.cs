using AutoMapper;
using BCrypt.Net;
using OfficeOpenXml.Packaging.Ionic.Zip;
using SalesTracker.Application.DTOs;
using SalesTracker.Application.Interfaces;
using SalesTracker.InfraStructure.Interfaces;
using SalesTracker.InfraStructure.Models.Entities;
using SalesTracker.Shared.Constants;
using SalesTracker.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesTracker.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;
        private readonly IRefreshTokenRepository _refreshTokenRepo;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepo, IRefreshTokenRepository refreshTokenRepo, ITokenService tokenService, IMapper mapper )
        {
            this._userRepo = userRepo;
            this._refreshTokenRepo = refreshTokenRepo;
            this._tokenService = tokenService;
            this._mapper = mapper;

        }

        public async Task<ReadUserDto> RegisterAsync(RegisterDto dto)
        {
            if (await _userRepo.UsernameExistsAsync(dto.Username))
                throw new Exception("Username already exists");

            var user = _mapper.Map<User>(dto);
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            user.IsActive = true;

            var created = await _userRepo.AddAsync(user);
            return _mapper.Map<ReadUserDto>(created);
        }

        public async Task<TokenResponseDto?> LoginAsync(LoginDto dto)
        {
            var user = await _userRepo.GetByUsernameAsync(dto.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return null;

            var accessToken = _tokenService.CreateAccessToken(user);
            var tokenEntity = _tokenService.CreateRefreshTokenEntity(user.Id);


            await _refreshTokenRepo.AddAsync(tokenEntity);

            return new TokenResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = tokenEntity.Token
            };
        }

        public async Task<TokenResponseDto?> RefreshTokenAsync(string refreshToken)
        {
            var tokenEntity = await _refreshTokenRepo.GetValidTokenAsync(refreshToken);
            if (tokenEntity == null)
                throw new AppException(APIMessages.ExpiredOrInvalidToken);

            var user = await _userRepo.GetByIdAsync(tokenEntity.UserId);
            if (user == null)
                throw new AppException(APIMessages.UnauthorizedAccess);

            var accessToken = _tokenService.CreateAccessToken(user);

            tokenEntity.UsageCount++;

            string refreshTokenToReturn;

            if (tokenEntity.UsageCount >= tokenEntity.MaxUsageCount)
            {
                // Revoke and rotate
                tokenEntity.IsRevoked = true;

                var newTokenEntity = _tokenService.CreateRefreshTokenEntity(user.Id);
                await _refreshTokenRepo.AddAsync(newTokenEntity);

                refreshTokenToReturn = newTokenEntity.Token;
            }
            else
            {
                // Reuse same refresh token
                refreshTokenToReturn = tokenEntity.Token;
            }
            await _refreshTokenRepo.UpdateAsync(tokenEntity);

            return new TokenResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenToReturn
            };
        }

    }
}
