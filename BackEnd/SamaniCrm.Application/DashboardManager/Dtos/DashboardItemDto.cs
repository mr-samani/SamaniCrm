using System.ComponentModel.DataAnnotations;
using System.Text.Json.Nodes;

namespace SamaniCrm.Application.DashboardManager;

public class DashboardItemDto
{
    public Guid? Id { get; set; }
    public Guid DashboardId { get; set; }
    public JsonObject Position { get; set; } = new JsonObject() { };
    [MaxLength(100)]
    public string? ComponentName { get; set; }
    [MaxLength(2000)]
    public string? Data { get; set; }

}