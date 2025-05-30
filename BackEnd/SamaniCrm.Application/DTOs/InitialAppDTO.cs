using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamaniCrm.Application.InitialApp.Queries;
using SamaniCrm.Core.Shared.DTOs;

namespace SamaniCrm.Application.DTOs
{
    public class InitialAppDTO
    {
        public List<LanguageDTO> Languages { get; set; } = [];
        public string DefaultLang { get; set; } = string.Empty;
        public bool RequireCaptcha { get; set; }
    }
}
