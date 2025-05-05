using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.DTOs;

namespace SamaniCrm.Application.Menu.Queries
{
    public record GetAllMenuItemsQuery : IRequest<List<MenuDTO>>;


    public class GetAllMenuItemsQueryHandler : IRequestHandler<GetAllMenuItemsQuery, List<MenuDTO>>
    {

        private readonly IApplicationDbContext _dbContext;

        public GetAllMenuItemsQueryHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<MenuDTO>> Handle(GetAllMenuItemsQuery request, CancellationToken cancellationToken)
        {
            var result = await _dbContext.Menus
                .Include(m => m.Localizations)
                .Include(c => c.Children)
                    .ThenInclude(l => l.Localizations)
                .Where(w => w.ParentId == null)
                .OrderBy(o => o.OrderIndex)
                .Select(s => MapToDto(s, "")).ToListAsync();
            return result ?? [];
        }



        private static MenuDTO MapToDto(SamaniCrm.Domain.Entities.Menu menu, string language)
        {
            var dto = new MenuDTO
            {
                Id = menu.Id,
                Code = menu.Code,
                Icon = menu.Icon,
                OrderIndex = menu.OrderIndex,
                ParentId = menu.ParentId,
                Target = menu.Target,
                Url = menu.Url,
                IsActive = menu.IsActive,
                Title = menu.Localizations?.FirstOrDefault(t => t.Culture == language)?.Value ?? menu.Code,
                Children = menu.Children?
                    // .Where(c => c.IsActive)
                    .OrderBy(c => c.OrderIndex)
                    .Select(c => MapToDto(c, language))
                    .ToList() ?? []
            };
            return dto;
        }

    }
}
