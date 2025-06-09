using SamaniCrm.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Domain.Entities.ProductEntities
{
    public class ProductFile : IAuditableEntity, ISoftDelete
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }

        public Guid FileId { get; set; } 
        [MaxLength(1000)]
        public string? Description { get; set; } 

        public virtual Product Product { get; set; } = default!;
        public virtual FileFolder File { get; set; } = default!;



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
