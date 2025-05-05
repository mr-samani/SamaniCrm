using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamaniCrm.Domain.Interfaces;

namespace SamaniCrm.Domain.Entities
{
    public class Menu : IAuditableEntity, ISoftDelete
    {
        public Guid Id { get; set; }
        /// <summary>
        /// Is Uniquie for localization key
        /// </summary>
        public required string Code { get; set; }
        [MaxLength(300)]
        public string? Icon { get; set; }
        [MaxLength(2000)]
        public string? Url { get; set; }

        public int OrderIndex { get; set; }

        public bool IsActive { get; set; } = true;

        public Guid? ParentId { get; set; }

        [MaxLength(100)]
        public MenuTargetEnum Target { get; set; } = MenuTargetEnum.Self;

        public virtual ICollection<Menu> Children { get; set; } = new List<Menu>();

        public virtual ICollection<Localization>? Localizations { get; set; }







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


    public enum MenuTargetEnum
    {
        [Description("_self")]
        Self,

        [Description("_blank")]
        Blank,

        [Description("_parent")]
        Parent,

        [Description("_top")]
        Top
    }

}
