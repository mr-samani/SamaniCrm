using MediatR;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Application.Product.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);

            if (menu == null)
                throw new NotFoundException("Menu not found.");


            List<MenuTranslationsDTO> translations = await _dbContext.Languages
              .GroupJoin(_dbContext.MenuTranslations.Where(w => w.MenuId == request.Id),
                lang => lang.Culture,
                translation => translation.Culture,
                (lang, trans) => new { lang, trans }
              )
              .SelectMany(
              x => x.trans.DefaultIfEmpty(),
              (x, trans) => new MenuTranslationsDTO()
              {
                  Culture = x.lang.Culture,
                  MenuId = menu.Id,
                  Title = trans != null ? trans.Title : "",

              }
              ).ToListAsync(cancellationToken);


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
