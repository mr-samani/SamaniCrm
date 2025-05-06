using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamaniCrm.Domain.Interfaces;

namespace SamaniCrm.Domain.Entities
{
    public class MenuTranslation : IAuditableEntity, ISoftDelete
    {
        public Guid Id { get; set; }
        public Guid MenuId { get; set; }
        [MaxLength(10)]
        public required string Culture { get; set; }

        public string Title { get; set; } = string.Empty!;

        public virtual Menu Menu { get; set; }
        public virtual Language Language { get; set; }






        // Implementing IAuditableEntity properties
        public DateTime CreationTime { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? LastModifiedTime { get; set; }
        public string? LastModifiedBy { get; set; }

        // Implementing ISoftDelete properties
        public bool IsDeleted { get; set; }
        public DateTime? DeletedTime { get; set; }
        public string? DeletedBy { get; set; }

    }
}
