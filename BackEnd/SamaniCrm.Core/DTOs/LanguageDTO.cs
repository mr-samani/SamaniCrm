using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Core.Shared.DTOs
{

    public class LanguageDTO
    {
        public required string Culture { get; set; } 
        public required string Name { get; set; }
        public bool IsRtl { get; set; } = false;
        public string? Flag { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
    }


}
