using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Domain.Entities.ProductEntities
{
    public class ProductFile
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }

        public string FileUrl { get; set; } = default!;
        public string FileType { get; set; } = default!;  // مثلا "pdf", "exe", "epub"

        public Product Product { get; set; } = default!;
    }

}
