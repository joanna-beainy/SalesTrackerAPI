using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesTracker.Shared.Settings
{
    public class AuthenticationSettings
    {
        public int AccessTokenTTLMinutes { get; set; }
        public int RefreshTokenMaxUses { get; set; }
        public int RefreshTokenTTL { get; set; }
    }

}
