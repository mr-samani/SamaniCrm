using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.DTOs
{
    public class UserDTO
    {
        public required Guid Id { get; set; }
        [Sortable]
        public required string UserName { get; set; }
        [Sortable]
        public required string FirstName { get; set; }
        [Sortable]
        public string LastName { get; set; } = string.Empty;
        [Sortable]
        public string FullName { get; set; } = string.Empty;
        [Sortable]
        public string Email { get; set; } = string.Empty;
        public string ProfilePicture { get; set; } = string.Empty;

        [Sortable]
        public string Lang { get; set; } = "";

        public string Address { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime? CreationTime { get; set; }

        public List<string> Roles { get; set; } = [];
        public List<string>? Permissions { get; set; }
        public string? GivenName { get; set; }
    }
}
