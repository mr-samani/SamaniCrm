using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
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
            MenuEntity menu;

            if (request.Id.HasValue)
            {
                menu = await _dbContext.Menus.FindAsync(request.Id.Value, cancellationToken);
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

            await _dbContext.SaveChangesAsync(cancellationToken);

            return menu.Id;
        }
    }


}
