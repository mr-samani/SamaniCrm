using MediatR;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SamaniCrm.Application.ProductManagerManager.Commands
{
    public record DeleteProductAttributeCommand(Guid Id) : IRequest<bool>;

    public class DeleteProductAttributeCommandHandler : IRequestHandler<DeleteProductAttributeCommand, bool>
    {
        private readonly IApplicationDbContext _dbContext;

        public DeleteProductAttributeCommandHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Handle(DeleteProductAttributeCommand request, CancellationToken cancellationToken)
        {
            var entity = await _dbContext.ProductAttributes.FindAsync(request.Id);
            if (entity == null)
                throw new NotFoundException("ProductAttribute not found.");

            entity.IsDeleted = true;
            entity.DeletedTime = DateTime.UtcNow;
            var result = await _dbContext.SaveChangesAsync(cancellationToken);
            return result > 0;
        }
    }
}
