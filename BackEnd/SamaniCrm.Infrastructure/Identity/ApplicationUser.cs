using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using SamaniCrm.Domain.Entities;
using SamaniCrm.Domain.Interfaces;

namespace SamaniCrm.Infrastructure.Identity;

// TODO: add base entity for autitable log
public class ApplicationUser: IdentityUser<Guid>
{
    public string? FirstName { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string? FullName { get; set; } = string.Empty;
    public string? Address { get; set; }
    // public string? PhoneNumber { get; set; }
    public string? ProfilePicture { get; set; }
    public required string Lang { get; set; }


    public virtual UserSetting UserSetting { get; set; } = new UserSetting();
}
