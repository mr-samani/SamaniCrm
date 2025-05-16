using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using SamaniCrm.Domain.Entities;
using SamaniCrm.Domain.Interfaces;

namespace SamaniCrm.Infrastructure.Identity
{
    public class ApplicationUser: IdentityUser<Guid>,IAuditableEntity,ISoftDelete
    {
        public string? FirstName { get; set; }
        public string LastName { get; set; } = string.Empty;
        public string? FullName { get; set; } = string.Empty;
        public string? Address { get; set; }
        // public string? PhoneNumber { get; set; }
        public string? ProfilePicture { get; set; }
        public required string Lang { get; set; }

        // Implementing IAuditableEntity properties
        public DateTime CreationTime { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? LastModifiedTime { get; set; }
        public string? LastModifiedBy { get; set; }

        // Implementing ISoftDelete properties
        public bool IsDeleted { get; set; }
        public DateTime? DeletedTime { get; set; }
        public string? DeletedBy { get; set; }

    }
}
