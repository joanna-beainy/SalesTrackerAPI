using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SalesTracker.Application.Interfaces;
using SalesTracker.InfraStructure.Models.Entities;
using SalesTracker.Shared.Settings;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SalesTracker.Application.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly AuthenticationSettings _authSettings;

        public TokenService(IConfiguration config, IOptions<AuthenticationSettings> options)
        {
            _config = config;
            _authSettings = options.Value;
        }

        public string CreateAccessToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role.Name)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_authSettings.AccessTokenTTLMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string CreateRefreshToken()
        {
            var bytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        public RefreshToken CreateRefreshTokenEntity(int userId)
        {
            return new RefreshToken
            {
                Token = CreateRefreshToken(),
                UserId = userId,
                UsageCount = 0,
                MaxUsageCount = _authSettings.RefreshTokenMaxUses,
                ExpiresAt = DateTime.UtcNow.AddDays(_authSettings.RefreshTokenTTL),
                IsRevoked = false
            };
        }
    }
}
