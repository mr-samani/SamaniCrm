using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using SamaniCrm.Domain.Constants;

namespace SamaniCrm.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string? FirstName { get; set; }
        public string LastName { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? ProfilePicture { get; set; }


        public ICollection<Roles> Roles { get; set; } = new List<Roles>();

    }
}
