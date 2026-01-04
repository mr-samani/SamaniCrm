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
using SamaniCrm.Domain.Entities;
using MenuEntity = SamaniCrm.Domain.Entities.Menu;

namespace SamaniCrm.Application.Menu.Commands
{
    public class CreateOrEditMenuCommand : MenuDTO, IRequest<Guid>
    { }


    public class CreateOrEditMenuCommandHandler : IRequestHandler<CreateOrEditMenuCommand, Guid>
    {
        private readonly IApplicationDbContext _dbContext;

        public CreateOrEditMenuCommandHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Guid> Handle(CreateOrEditMenuCommand request, CancellationToken cancellationToken)
        {
            MenuEntity? menu = null;

            if (request.Id != null)
            {
                menu = await _dbContext.Menus
                     .Include(p => p.Translations)
                     .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
                if (menu == null)
                    throw new NotFoundException("Menu not found.");
            }
            else
            {
                menu = new MenuEntity();
                _dbContext.Menus.Add(menu);
            }

            menu.Icon = request.Icon;
            menu.Url = request.Url;
            menu.OrderIndex = request.OrderIndex;
            menu.IsActive = request.IsActive;
            menu.ParentId = request.ParentId;
            menu.Target = request.Target;
            menu.LastModifiedTime = DateTime.UtcNow;
            // نباید مقدار سیستمی توسط برنامه قایل تغییر باشد 
            // menu.IsSystem = request.IsSystem;
            // Update translations
            foreach (var item in request.Translations ?? [])
            {
                var existingTranslation = menu.Translations?
                    .FirstOrDefault(t => t.Culture == item.Culture);

                if (existingTranslation != null)
                {
                    // Update existing translation
                    existingTranslation.Title = item.Title;
                }
                else
                {
                    // Add new translation
                    menu.Translations?.Add(new MenuTranslation
                    {
                        Culture = item.Culture,
                        Title = item.Title,
                    });
                }
            }
            await _dbContext.SaveChangesAsync(cancellationToken);

            return menu.Id;
        }
    }


}
