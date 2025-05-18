using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Domain.Entities.ProductEntities
{
    public class ProductTranslation:TranslationBaseClass
    {
        public Guid ProductId { get; set; }
        [MaxLength(250)]
        public string Title { get; set; } = default!;

        [MaxLength(1000)]
        public string? Description { get; set; }

        public virtual Product Product { get; set; } = default!;

    }
}
