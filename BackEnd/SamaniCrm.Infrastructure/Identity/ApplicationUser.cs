using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using SamaniCrm.Domain.Entities;
using SamaniCrm.Domain.Interfaces;

namespace SamaniCrm.Infrastructure.Identity;



public class ApplicationUser : IdentityUser<Guid>, IMayHaveTenant, IAuditedEntity
{
    // ─── IMayHaveTenant ───
    public Guid? TenantId { get; set; }

    public string? FirstName { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string? FullName { get; set; } = string.Empty;
    public string? Address { get; set; }
    // public string? PhoneNumber { get; set; }
    public string? ProfilePicture { get; set; }
    public required string Lang { get; set; }


    public virtual UserSetting UserSetting { get; set; } = new UserSetting();

    // ─── IAuditedEntity ───
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid? CreatedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public Guid? ModifiedBy { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }
    public bool IsDeleted { get; set; } = false;
    public byte[]? RowVersion { get; set; }


    public virtual ICollection<ApplicationRole> Roles { get; set; } = [];

}
