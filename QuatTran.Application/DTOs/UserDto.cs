using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuatTran.Application.DTOs
{
    public class UserDto
    {

        public int UserId { get; set; }

        public string FullName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string PasswordHash { get; set; } = null!;

        public string? Phone { get; set; }

        public string? Address { get; set; }

        public string? Role { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}
