using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Interfaces;

namespace SamaniCrm.Application.Localize.Queries
{
    public record class GetAllLanguageKeys(string culture) : IRequest<Dictionary<string, string?>>;

    public class GetAllLanguageKeysHandler : IRequestHandler<GetAllLanguageKeys, Dictionary<string, string?>>
    {
        private readonly IApplicationDbContext _dbContext;

        public GetAllLanguageKeysHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Dictionary<string, string?>> Handle(GetAllLanguageKeys request, CancellationToken cancellationToken)
        {
            Dictionary<string, string?> result = await _dbContext.Localizations
                 .Where(x => x.Culture == request.culture)
                 .ToDictionaryAsync(x => x.Key, x => x.Value);
            return result;
        }
    }


}
