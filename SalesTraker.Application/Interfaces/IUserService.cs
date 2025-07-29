using SalesTracker.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesTracker.Application.Interfaces
{
    public interface IUserService
    {
        Task<TokenResponseDto?> LoginAsync(LoginDto dto);
        Task<ReadUserDto> RegisterAsync(RegisterDto dto);
        Task<TokenResponseDto?> RefreshTokenAsync(string refreshToken);


    }

}
