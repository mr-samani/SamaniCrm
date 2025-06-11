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
    public record GetAllActiveLanguageQuery() : IRequest<List<LanguageDTO>>;

    public class GetAllActiveLanguageQueryHandler : IRequestHandler<GetAllActiveLanguageQuery, List<LanguageDTO>>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ILanguageService _languageService;

        public GetAllActiveLanguageQueryHandler(IApplicationDbContext dbContext, ILanguageService languageService)
        {
            _dbContext = dbContext;
            _languageService = languageService;
        }

        public async Task<List<LanguageDTO>> Handle(GetAllActiveLanguageQuery request, CancellationToken cancellationToken)
        {
            var result = await _languageService.GetAllActiveLanguages();
            return result;
        }
    }
}
