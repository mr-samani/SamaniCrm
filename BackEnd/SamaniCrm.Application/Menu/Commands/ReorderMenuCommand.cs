using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Interfaces;

namespace SamaniCrm.Application.Menu.Commands
{
    public record ReorderMenuCommand(List<ReorderItem> Items) : IRequest<bool>;

    public record ReorderItem(Guid MenuId, Guid? ParentId, int OrderIndex);
    public class ReorderMenuCommandHandler : IRequestHandler<ReorderMenuCommand, bool>
    {
        private readonly IApplicationDbContext _dbContext;

        public ReorderMenuCommandHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Handle(ReorderMenuCommand request, CancellationToken cancellationToken)
        {
            var menuIds = request.Items.Select(i => i.MenuId).ToList();
            var menus = await _dbContext.Menus.Where(m => menuIds.Contains(m.Id)).ToListAsync(cancellationToken);

            foreach (var item in request.Items)
            {
                var menu = menus.FirstOrDefault(m => m.Id == item.MenuId);
                if (menu != null)
                {
                    menu.OrderIndex = item.OrderIndex;
                    menu.LastModifiedTime = DateTime.UtcNow;
                    menu.ParentId = item.ParentId;
                }
            }

            var result = await _dbContext.SaveChangesAsync(cancellationToken);
            return result > 0;
        }
    }

}
