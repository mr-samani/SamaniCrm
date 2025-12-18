using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using SamaniCrm.Domain.Interfaces;

namespace SamaniCrm.Domain.Entities.Dashboard
{
    public class DashboardItem : IAuditableEntity
    {
        public Guid Id { get; set; }
        public Guid DashboardId { get; set; }
        public virtual Dashboard Dashboard { get; set; } = default!;
        [MaxLength(500)]
        public string Position { get; set; } = "";
        [MaxLength(100)]
        public string? ComponentName { get; set; }
        [MaxLength(2000)]
        public string? Data { get; set; }


        // Implementing IAuditableEntity properties
        public DateTime CreationTime { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? LastModifiedTime { get; set; }
        public string? LastModifiedBy { get; set; }

    }
}