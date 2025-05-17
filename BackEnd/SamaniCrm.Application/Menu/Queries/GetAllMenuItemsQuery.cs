using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Core.Shared.Interfaces;
using MenuEntity = SamaniCrm.Domain.Entities.Menu;



namespace SamaniCrm.Application.Menu.Queries
{
    public record GetAllMenuItemsQuery : IRequest<List<MenuDTO>>;


    public class GetAllMenuItemsQueryHandler : IRequestHandler<GetAllMenuItemsQuery, List<MenuDTO>>
    {

        private readonly IApplicationDbContext _dbContext;
        private readonly ILocalizer L;

        public GetAllMenuItemsQueryHandler(IApplicationDbContext dbContext, ILocalizer l)
        {
            _dbContext = dbContext;
            L = l;
        }

        public async Task<List<MenuDTO>> Handle(GetAllMenuItemsQuery request, CancellationToken cancellationToken)
        {
            var currentLanguage = L.CurrentLanguage;
            var allMenus = await _dbContext.Menus
                                .Include(m => m.Translations)
                                .Include(m => m.Children)
                                    .ThenInclude(c => c.Translations)
                                .OrderBy(m => m.OrderIndex)
                                .ToListAsync();
            var rootMenus = allMenus.Where(m => m.ParentId == null).ToList();
            var result = rootMenus.Select(m => MapToDtoRecursive(m, currentLanguage)).ToList();
            return result ?? [];
        }



        private static MenuDTO MapToDtoRecursive(MenuEntity menu, string language)
        {
            return new MenuDTO
            {
                Id = menu.Id,
                Icon = menu.Icon,
                OrderIndex = menu.OrderIndex,
                ParentId = menu.ParentId,
                Target = menu.Target,
                Url = menu.Url,
                IsActive = menu.IsActive,
                Title = menu.Translations?.FirstOrDefault(t => t.Culture == language)?.Title ?? "",
                Children = menu.Children?
                    .OrderBy(c => c.OrderIndex)
                    .Select(c => MapToDtoRecursive(c, language))
                    .ToList() ?? []
            };
        }

    }
}
