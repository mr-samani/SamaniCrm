using System.ComponentModel.DataAnnotations;

namespace SamaniCrm.Domain.Entities;

public class Dashboard : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public required string Title { get; set; }
    public int Order { get; set; }
    public Guid? UserId { get; set; }
    public bool IsPublic { get; set; }
    public virtual ICollection<DashboardItem> DashboardItems { get; set; } = [];
}