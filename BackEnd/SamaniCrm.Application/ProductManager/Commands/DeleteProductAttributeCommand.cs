using MediatR;
using Microsoft.EntityFrameworkCore;
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

            var now = DateTime.Now;

            entity.IsDeleted = true;
            entity.DeletedTime = now;

            var translations = await _dbContext.ProductAttributeTranslations
            .Where(x => x.ProductAttributeId == request.Id && !x.IsDeleted)
            .ToListAsync(cancellationToken);

            foreach (var translation in translations)
            {
                translation.IsDeleted = true;
                translation.DeletedTime = now;
            }


            var attrValues = await _dbContext.ProductAttributeValues
            .Where(x => x.AttributeId == request.Id && !x.IsDeleted)
            .ToListAsync(cancellationToken);

            foreach (var attr in attrValues)
            {
                attr.IsDeleted = true;
                attr.DeletedTime = now;
            }


            var result = await _dbContext.SaveChangesAsync(cancellationToken);
            return result > 0;
        }
    }
}
