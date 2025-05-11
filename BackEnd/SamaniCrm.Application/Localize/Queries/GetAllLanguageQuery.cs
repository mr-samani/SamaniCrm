using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Core.Shared.DTOs;
using SamaniCrm.Core.Shared.Interfaces;

namespace SamaniCrm.Application.Localize.Queries
{
    public record GetAllLanguageQuery() : IRequest<List<LanguageDTO>>;

    public class GetAllLanguageQueryHandler : IRequestHandler<GetAllLanguageQuery, List<LanguageDTO>>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ILanguageService _languageService;

        public GetAllLanguageQueryHandler(IApplicationDbContext dbContext, ILanguageService languageService)
        {
            _dbContext = dbContext;
            _languageService = languageService;
        }

        public async Task<List<LanguageDTO>> Handle(GetAllLanguageQuery request, CancellationToken cancellationToken)
        {
            var result = await _languageService.GetAllLanguagesForAdmin();
            return result;
        }
    }
}
