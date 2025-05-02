using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.DTOs
{

    public class LanguageDTO
    {
        public string Culture { get; set; } = default!;
        public string Name { get; set; } = default!;
        public bool IsRtl { get; set; } = false;
        public string? Flag { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
    }


}
