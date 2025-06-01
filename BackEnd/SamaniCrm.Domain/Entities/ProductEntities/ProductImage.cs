using SamaniCrm.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Domain.Entities.ProductEntities
{
    public class ProductImage: IAuditableEntity, ISoftDelete
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }

        public string Url { get; set; } = default!;
        public bool IsMain { get; set; }
        public int SortOrder { get; set; }

        public Product Product { get; set; } = default!;


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
