using MediatR;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SamaniCrm.Application.ProductManagerManager.Commands
{
    public record DeleteProductCommand(Guid Id) : IRequest<bool>;

    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, bool>
    {
        private readonly IApplicationDbContext _dbContext;
        public DeleteProductCommandHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var entity = await _dbContext.Products.FindAsync(request.Id);
            if (entity == null)
                throw new NotFoundException("Product not found.");


            var now = DateTime.UtcNow;

            entity.IsDeleted = true;
            entity.DeletedTime = now;

            var translations = await _dbContext.ProductTranslations
                .Where(x => x.ProductId == request.Id && !x.IsDeleted)
                .ToListAsync(cancellationToken);

            foreach (var translation in translations)
            {
                translation.IsDeleted = true;
                translation.DeletedTime = now;
            }

            var result = await _dbContext.SaveChangesAsync(cancellationToken);
            return result > 0;
        }
    }
}
