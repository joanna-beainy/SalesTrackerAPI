using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesTracker.Application.DTOs
{
    public class ReadUserDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string RoleName { get; set; }
    }

}
