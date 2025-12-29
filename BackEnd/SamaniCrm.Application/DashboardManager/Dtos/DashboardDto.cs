using System.ComponentModel.DataAnnotations;

namespace SamaniCrm.Application.DashboardManager;

public class DashboardDto
{
    public Guid? Id { get; set; }
    public Guid? UserId { get; set; }
    [Required]
    [MaxLength(100)]
    public required string Title { get; set; }
    public int Order { get; set; }
    public bool IsPublic { get; set; }
    public virtual ICollection<DashboardItemDto> DashboardItems { get; set; } = [];
}
