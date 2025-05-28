using MediatR;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.ProductManagerManager.Dtos;
using SamaniCrm.Domain.Entities.ProductEntities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SamaniCrm.Application.ProductManagerManager.Commands
{
    public class CreateOrUpdateProductAttributeCommandHandler : IRequestHandler<CreateOrUpdateProductAttributeCommand, Guid>
    {
        private readonly IApplicationDbContext _dbContext;

        public CreateOrUpdateProductAttributeCommandHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Guid> Handle(CreateOrUpdateProductAttributeCommand request, CancellationToken cancellationToken)
        {
            ProductAttribute entity;
            if (request.Id.HasValue)
            {
                entity = await _dbContext.ProductAttributes
                    .Include(x => x.Translations)
                    .FirstOrDefaultAsync(x => x.Id == request.Id.Value, cancellationToken);
                if (entity == null)
                    throw new NotFoundException("ProductAttribute not found.");
            }
            else
            {
                entity = new ProductAttribute();
                _dbContext.ProductAttributes.Add(entity);
            }

            entity.ProductTypeId = request.ProductTypeId;
            entity.DataType = request.DataType;
            entity.IsRequired = request.IsRequired;
            entity.IsVariant = request.IsVariant;
            entity.SortOrder = request.SortOrder;
            entity.LastModifiedTime = DateTime.UtcNow;

            // Handle translations
            if (request.Translations != null)
            {
                entity.Translations.Clear();
                foreach (var t in request.Translations)
                {
                    entity.Translations.Add(new ProductAttributeTranslation
                    {
                        ProductAttributeId = entity.Id,
                        Culture = t.Culture,
                        Name = t.Name,
                        CreationTime = DateTime.UtcNow
                    });
                }
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
            return entity.Id;
        }
    }
}
