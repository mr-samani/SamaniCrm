using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Domain.Entities
{
    public class TranslationBaseClass
    {
        public Guid Id { get; set; }

        [MaxLength(10)]
        public required string Culture { get; set; }
        public virtual Language Language { get; set; } = default!;


    }
}
