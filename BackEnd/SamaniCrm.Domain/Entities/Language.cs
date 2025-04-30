using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamaniCrm.Domain.Interfaces;

namespace SamaniCrm.Domain.Entities
{
    public class Language : IAuditableEntity, ISoftDelete
    {
        [MaxLength(10)]
        public string Culture { get; set; } = default!;
        [MaxLength(100)]
        public string Name { get; set; } = default!;
        public bool IsRtl { get; set; } = false;
        public string? Flag { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<Localization>? Localizations { get; set; } = new List<Localization>();




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
