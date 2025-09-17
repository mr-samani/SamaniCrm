using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Core.Shared.Consts;
using SamaniCrm.Core.Shared.Interfaces;
using SamaniCrm.Domain.Entities;

namespace SamaniCrm.Application.Localize.Queries
{
    public record class GetFrontEndLocalizationsCommand(string culture) : IRequest<Dictionary<string, string>>;

    public class GetFrontEndLocalizationsCommandHandler : IRequestHandler<GetFrontEndLocalizationsCommand, Dictionary<string, string>>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ICacheService _cacheService;

        public GetFrontEndLocalizationsCommandHandler(IApplicationDbContext dbContext, ICacheService cacheService)
        {
            _dbContext = dbContext;
            _cacheService = cacheService;
        }

        public async Task<Dictionary<string, string>> Handle(GetFrontEndLocalizationsCommand request, CancellationToken cancellationToken)
        {
            var cacheKey = CacheKeys.FrontEndLocalize_ + request.culture;
            var result = await _cacheService.GetAsync<Dictionary<string, string>>(cacheKey);
            if (result == null)
            {
                result = await _dbContext.Localizations
                   .Select(s => new LocalizationKeyDTO()
                   {
                       Key = s.Key,
                       Culture = s.Culture,
                       Category = s.Category,
                       Id = s.Id,
                       Value = s.Value ?? ""
                   }).Where(x => x.Culture == request.culture && x.Category == LocalizationCategoryEnum.Frontend)
                    .ToDictionaryAsync(x => x.Key, v => v.Value);
                await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromDays(30));
            }
            return result;
        }
    }


}
