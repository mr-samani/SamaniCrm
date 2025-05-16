using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamaniCrm.Domain.Interfaces;

namespace SamaniCrm.Domain.Entities
{
    public class Localization : IAuditableEntity, ISoftDelete
    {
        public Guid Id { get; set; }
        [MaxLength(10)]
        public required string Culture { get; set; }
        [MaxLength(250)]
        public required string Key { get; set; }
        [MaxLength(2000)]
        public string? Value { get; set; }

        [MaxLength(100)]
        public LocalizationCategoryEnum Category { get; set; } = LocalizationCategoryEnum.App;


        public Language? Language { get; set; }



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



    public enum LocalizationCategoryEnum
    {
        [Description("Role")]
        Role = 0,
        [Description("Permission")]
        Permission,
        [Description("Backend")]
        Backend,
        [Description("Frontend")]
        Frontend,
        [Description("Enum")]
        Enum,
        [Description("App")]
        App,
        [Description("Other")]
        Other
    }
}
