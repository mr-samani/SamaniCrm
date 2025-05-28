using MediatR;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SamaniCrm.Application.ProductManagerManager.Commands
{
    public record DeleteProductTypeCommand(Guid Id) : IRequest<bool>;

    public class DeleteProductTypeCommandHandler : IRequestHandler<DeleteProductTypeCommand, bool>
    {
        private readonly IApplicationDbContext _dbContext;
        public DeleteProductTypeCommandHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<bool> Handle(DeleteProductTypeCommand request, CancellationToken cancellationToken)
        {
            var entity = await _dbContext.ProductTypes.FindAsync(request.Id);
            if (entity == null)
                throw new NotFoundException("ProductType not found.");
            entity.IsDeleted = true;
            entity.DeletedTime = DateTime.UtcNow;
            var result = await _dbContext.SaveChangesAsync(cancellationToken);
            return result > 0;
        }
    }
}
