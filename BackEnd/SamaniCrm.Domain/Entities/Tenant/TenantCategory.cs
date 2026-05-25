using System.ComponentModel.DataAnnotations;

namespace SamaniCrm.Domain.Entities;

public class TenantCategory : BaseEntity
{
    public Guid TenantId { get; set; }


    [MaxLength(200)]
    public required string Name { get; set; }

    [MaxLength(2000)]
    public string? Description { get; set; }
    public Guid? ParentId { get; set; }
    public int SortOrder { get; set; }

    public bool IsActive { get; set; } = true;

    public virtual Tenant Tenant { get; set; } = default!;
}