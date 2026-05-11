using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;
using System.Text.Json.Nodes;

namespace SamaniCrm.Domain.Entities;

public class DashboardItem : BaseEntity
{
    public Guid DashboardId { get; set; }
    public virtual Dashboard Dashboard { get; set; } = default!;
    [MaxLength(500)]
    public string Position { get; set; } = "";
    [MaxLength(100)]
    public string? ComponentName { get; set; }
    [MaxLength(2000)]
    public string? Data { get; set; }
}