using SalesTracker.InfraStructure.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesTracker.InfraStructure.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task AddAsync(RefreshToken token);
        Task<RefreshToken?> GetValidTokenAsync(string token);
        Task RevokeTokenAsync(int tokenId);

        Task UpdateAsync(RefreshToken token);
    }

}
