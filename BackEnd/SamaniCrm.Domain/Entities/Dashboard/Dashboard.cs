using System.ComponentModel.DataAnnotations;
using SamaniCrm.Domain.Interfaces;

namespace SamaniCrm.Domain.Entities.Dashboard
{
    public class Dashboard : IAuditableEntity
    {
        public Guid Id { get; set; }
        [Required]
        [MaxLength(100)]
        public required string Title { get; set; }
        public int Order { get; set; }
        public Guid? UserId { get; set; }
        public bool IsPublic { get; set; }
        public virtual ICollection<DashboardItem> DashboardItems { get; set; } = [];


        // Implementing IAuditableEntity properties
        public DateTime CreationTime { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? LastModifiedTime { get; set; }
        public string? LastModifiedBy { get; set; }

    }
}