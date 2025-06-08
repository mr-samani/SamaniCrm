using SamaniCrm.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Domain.Entities
{
    public class FileFolder : IAuditableEntity, ISoftDelete
    {
        public Guid Id { get; set; }
        public Guid? ParentId { get; set; }

        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string RelativePath { get; set; } = string.Empty;


        [MaxLength(100)]
        public string? Icon { get; set; }
        public bool IsFolder { get; set; }
        public bool IsPublic { get; set; }
        public bool IsStatic { get; set; } = false;
        public string? Extension { get; set; }
        public string? ContentType { get; set; }
        public decimal? ByteSize { get; set; }
        public string? Thumbnails { get; set; }

        public virtual ICollection<FileFolder> Children { get; set; } = [];
        public virtual FileFolder? Parent {  get; set; }




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
