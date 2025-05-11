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
        public required string LastName { get; set; } = string.Empty;
        [Sortable]
        public required string FullName { get; set; } = string.Empty;
        [Sortable]
        public required string Email { get; set; } = string.Empty;
        public required string ProfilePicture { get; set; } = string.Empty;

        [Sortable]
        public required string  Lang { get; set; }

        public string Address { get; set; } = string.Empty;
        public required string PhoneNumber { get; set; } = string.Empty;
        public required DateTime CreationTime { get; set; }

        public required List<string> Roles { get; set; }

    }
}
