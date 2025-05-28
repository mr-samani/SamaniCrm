using MediatR;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.ProductManagerManager.Dtos;
using SamaniCrm.Domain.Entities;
using SamaniCrm.Domain.Entities.ProductEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.ProductManagerManager.Commands
{
    public class CreateOrUpdateProductCategoryCommand:ProductCategoryDto,IRequest<Guid>
    {
    }

    public class CreateOrUpdateProductCategoryCommandHandler : IRequestHandler<CreateOrUpdateProductCategoryCommand, Guid>
    {
        private readonly IApplicationDbContext _dbContext;

        public CreateOrUpdateProductCategoryCommandHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Guid> Handle(CreateOrUpdateProductCategoryCommand request, CancellationToken cancellationToken)
        {
            ProductCategory cat;

            if (request.Id.HasValue)
            {
                cat = await _dbContext.ProductCategories
                     .Include(p => p.Translations)
                     .FirstOrDefaultAsync(p => p.Id == request.Id.Value, cancellationToken);
                if (cat == null)
                    throw new NotFoundException("Menu not found.");
            }
            else
            {
                cat = new ProductCategory();
                _dbContext.ProductCategories.Add(cat);
            }

            cat.Image = request.Image;
            cat.OrderIndex   = request.OrderIndex;
            cat.Slug = request.Slug;
            cat.IsActive = request.IsActive;
            cat.ParentId = request.ParentId;

            cat.LastModifiedTime = DateTime.UtcNow;
            foreach (var item in request.Translations ?? [])
            {
                var existingTranslation = cat.Translations
                    .FirstOrDefault(t => t.Culture == item.Culture);

                if (existingTranslation != null)
                {
                    // Update existing translation
                    existingTranslation.Title = item.Title;
                    existingTranslation.Description = item.Description;
                }
                else
                {
                    // Add new translation
                    cat.Translations.Add(new ProductCategoryTranslation
                    {
                        Culture = item.Culture,
                        Title = item.Title,
                        Description= item.Description,
                    });
                }
            }
            await _dbContext.SaveChangesAsync(cancellationToken);

            return cat.Id;
        }
    }
}
