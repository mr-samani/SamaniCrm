using MediatR;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Application.ProductManagerManager.Dtos;
using SamaniCrm.Core.Shared.DTOs;
using SamaniCrm.Core.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.MenuQueries
{
    public record GetMenuForEditQuery(Guid Id) : IRequest<MenuDTO>;
    public class GetMenuForEditQueryHandler : IRequestHandler<GetMenuForEditQuery, MenuDTO>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ILanguageService _languageService;

        public GetMenuForEditQueryHandler(
            IApplicationDbContext dbContext,
            ILanguageService languageService)
        {
            _dbContext = dbContext;
            _languageService = languageService;
        }

        public async Task<MenuDTO> Handle(GetMenuForEditQuery request, CancellationToken cancellationToken)
        {
            var menu = await _dbContext.Menus
         .AsNoTracking()
         .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);

            if (menu == null)
                throw new NotFoundException("Menu not found.");

            var allLangs = await _languageService.GetAllActiveLanguages();

            var translationDict = await _dbContext.MenuTranslations
                .AsNoTracking()
                .Where(x => x.MenuId == request.Id)
                .Select(x => new
                {
                    x.Culture,
                    x.Title
                })
                .ToDictionaryAsync(x => x.Culture, x => x.Title, cancellationToken);


            var translations = allLangs.Select(lang => new MenuTranslationsDTO
            {
                Culture = lang.Culture,
                MenuId = menu.Id,
                Title = translationDict.TryGetValue(lang.Culture, out var title) ? title : string.Empty
            }).ToList();

            return new MenuDTO
            {
                Id = menu.Id,
                Name = menu.Name,
                Icon = menu.Icon,
                Url = menu.Url,
                OrderIndex = menu.OrderIndex,
                IsActive = menu.IsActive,
                ParentId = menu.ParentId,
                Target = menu.Target,
                Translations = translations
            };
        }
    }

}
