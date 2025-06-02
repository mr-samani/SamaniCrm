using MediatR;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.ProductManagerManager.Dtos;
using SamaniCrm.Domain.Entities.ProductEntities;
using System;

namespace SamaniCrm.Application.ProductManagerManager.Commands
{
    public class CreateOrUpdateProductTypeCommand : ProductTypeDto, IRequest<Guid>
    {
    }



    public class CreateOrUpdateProductTypeCommandHandler : IRequestHandler<CreateOrUpdateProductTypeCommand, Guid>
    {
        private readonly IApplicationDbContext _dbContext;
        public CreateOrUpdateProductTypeCommandHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Guid> Handle(CreateOrUpdateProductTypeCommand request, CancellationToken cancellationToken)
        {
            ProductType entity;
            if (request.Id.HasValue)
            {
                entity = await _dbContext.ProductTypes
                    .Include(x => x.Attributes)
                    .FirstOrDefaultAsync(x => x.Id == request.Id.Value, cancellationToken);
                if (entity == null)
                    throw new NotFoundException("ProductType not found.");
            }
            else
            {
                entity = new ProductType();
                _dbContext.ProductTypes.Add(entity);
            }
            entity.LastModifiedTime = DateTime.UtcNow;
            // Handle translations
            if (request != null)
            {
                var toRemove = entity.Translations.Where(t => !(request.Translations.Any(rt => rt.Culture == t.Culture))).ToList();
                foreach (var t in toRemove)
                    entity.Translations.Remove(t);
                foreach (var item in request.Translations ?? [])
                {
                    var existingTranslation = entity.Translations
                        .FirstOrDefault(t => t.Culture == item.Culture);

                    if (existingTranslation != null)
                    {
                        // Update existing translation
                        existingTranslation.Name = item.Name;
                        existingTranslation.Description = item.Description;
                    }
                    else
                    {
                        // Add new translation
                        entity.Translations.Add(new ProductTypeTranslation
                        {
                            Culture = item.Culture,
                            Name = item.Name,
                            Description = item.Description,
                        });
                    }
                }
            }
            await _dbContext.SaveChangesAsync(cancellationToken);
            return entity.Id;
        }
    }


}
