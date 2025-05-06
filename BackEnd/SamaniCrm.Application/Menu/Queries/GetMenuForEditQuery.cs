using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.DTOs;

namespace SamaniCrm.Application.Menu.Queries
{
    public record GetMenuForEditQuery(Guid Id) : IRequest<MenuDTO>;
    public class GetMenuForEditQueryHandler : IRequestHandler<GetMenuForEditQuery, MenuDTO>
    {
        private readonly IApplicationDbContext _dbContext;

        public GetMenuForEditQueryHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<MenuDTO> Handle(GetMenuForEditQuery request, CancellationToken cancellationToken)
        {
            var menu = await _dbContext.Menus
                .Include(m => m.Translations)
                .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);

            if (menu == null)
                throw new NotFoundException("Menu not found.");

            List<MenuTranslationsDTO> translations = await _dbContext.MenuTranslations
                .Select(s => new MenuTranslationsDTO()
                {
                    Culture = s.Culture,
                    Title = s.Title,
                    MenuId = s.MenuId,
                }).ToListAsync();


            return new MenuDTO
            {
                Id = menu.Id,
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
