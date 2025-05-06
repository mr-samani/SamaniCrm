using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.DTOs;

namespace SamaniCrm.Application.Localize.Queries
{
    public record class GetAllLanguageKeys(string culture) : IRequest<List<LocalizationKeyDTO>>;

    public class GetAllLanguageKeysHandler : IRequestHandler<GetAllLanguageKeys, List<LocalizationKeyDTO>>
    {
        private readonly IApplicationDbContext _dbContext;

        public GetAllLanguageKeysHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<LocalizationKeyDTO>> Handle(GetAllLanguageKeys request, CancellationToken cancellationToken)
        {
            var result = await _dbContext.Localizations
                .Select(s => new LocalizationKeyDTO()
                {
                    Key = s.Key,
                    Culture = s.Culture,
                    Category = s.Category,
                    Id = s.Id,
                    Value = s.Value ?? ""
                })
                 .Where(x => x.Culture == request.culture)
                 .ToListAsync();
            return result;
        }
    }


}
