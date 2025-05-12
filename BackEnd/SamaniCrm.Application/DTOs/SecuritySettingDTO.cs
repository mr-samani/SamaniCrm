using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.DTOs
{
    public class SecuritySettingDTO
    {
        public required PasswordComplexityDTO PasswordComplexity { get; set; }
    }
}
