using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesTracker.Application.DTOs
{
    public class RegisterDto
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public int RoleId { get; set; }
    }
}
