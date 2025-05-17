using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Domain.Entities.ProductEntities
{
    public class ProductCategoryTranslation : TranslationBaseClass
    {
        public Guid CategoryId { get; set; }
        [MaxLength(250)]
        public string Name { get; set; } = default!;
        [MaxLength(1000)]
        public string? Description { get; set; }

        public virtual ProductCategory ProductCategory { get; set; } = default!;
    }
}
