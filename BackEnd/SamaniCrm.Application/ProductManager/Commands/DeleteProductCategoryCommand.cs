using MediatR;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.ProductManagerManager.Commands
{
    public record DeleteProductCategoryCommand(Guid Id) :IRequest<bool>;


    public class DeleteProductCategoryCommandHandler : IRequestHandler<DeleteProductCategoryCommand, bool>
    {
        private readonly IApplicationDbContext _dbContext;

        public DeleteProductCategoryCommandHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Handle(DeleteProductCategoryCommand request, CancellationToken cancellationToken)
        {
            var menu = await _dbContext.ProductCategories.FindAsync(request.Id);
            if (menu == null)
                throw new NotFoundException("ProductCategory not found.");

            menu.IsDeleted = true;
            menu.DeletedTime = DateTime.UtcNow;

            var result = await _dbContext.SaveChangesAsync(cancellationToken);

            return result > 0;
        }
    }
}
