using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.DTOs
{
    public class UserResponseDTO
    {
        public required Guid Id { get; set; }
        [Sortable]
        public string? UserName { get; set; }
        [Sortable]
        public string? FirstName { get; set; }
        [Sortable]
        public string? LastName { get; set; } = string.Empty;
        [Sortable]
        public string? FullName { get; set; } = string.Empty;
        [Sortable]
        public string? Email { get; set; } = string.Empty;
        public string? ProfilePicture { get; set; } = string.Empty;


    }
}
