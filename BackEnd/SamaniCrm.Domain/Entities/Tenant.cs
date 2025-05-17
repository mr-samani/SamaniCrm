using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamaniCrm.Domain.Entities.ProductEntities;

namespace SamaniCrm.Domain.Entities
{
    public class Tenant
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string Slug { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; } = true;

      //  public ICollection<Product> Products { get; set; } = new List<Product>();
        public ICollection<ProductCategory> Categories { get; set; } = new List<ProductCategory>();
    }

}
