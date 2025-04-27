using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.DTOs
{
    public class CaptchaDto
    {
        public required string Key { get; set; }
        public required string Img { get; set; }
        public required bool Sensitive { get; set; } = false;
    }
}
