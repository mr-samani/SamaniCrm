using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamaniCrm.Core.Shared.DTOs;

namespace SamaniCrm.Core.Shared.Interfaces
{
    public interface ILanguageService
    {
        Task<List<LanguageDTO>> GetAllActiveLanguages();


        Task<List<LanguageDTO>> GetAllLanguagesForAdmin();

    }
}
