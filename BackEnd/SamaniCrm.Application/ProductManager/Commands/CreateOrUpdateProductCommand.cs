using MediatR;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.ProductManagerManager.Dtos;
using SamaniCrm.Domain.Entities.ProductEntities;
using System;

namespace SamaniCrm.Application.ProductManagerManager.Commands
{
    public class CreateOrUpdateProductCommand : ProductDto, IRequest<Guid>
    {
    }



    public class CreateOrUpdateProductCommandHandler : IRequestHandler<CreateOrUpdateProductCommand, Guid>
    {
        private readonly IApplicationDbContext _dbContext;
        public CreateOrUpdateProductCommandHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Guid> Handle(CreateOrUpdateProductCommand request, CancellationToken cancellationToken)
        {
            Product entity;
            if (request.Id.HasValue)
            {
                entity = await _dbContext.Products
                    .Include(x => x.Translations)
                    .Include(x => x.Images)
                    .Include(x => x.Files)
                    .Include(x => x.Prices)
                    .Include(x => x.AttributeValues)
                    .FirstOrDefaultAsync(x => x.Id == request.Id.Value, cancellationToken);
                if (entity == null)
                    throw new NotFoundException("Product not found.");
            }
            else
            {
                entity = new Product();
                _dbContext.Products.Add(entity);
            }
            entity.TenantId = request.TenantId;
            entity.CategoryId = request.CategoryId;
            entity.ProductTypeId = request.ProductTypeId;
            entity.Slug = request.Slug;
            entity.IsActive = request.IsActive;
            entity.LastModifiedTime = DateTime.UtcNow;

            // Handle SKU
            // اگر نیاز به تبدیل خاصی برای SKU دارید اینجا انجام دهید
            // entity.SKU = new Sku(request.SKU); // اگر Sku یک ValueObject باشد

            // Handle Translations
            if (request.Translations != null)
            {
                entity.Translations.Clear();
                foreach (var t in request.Translations)
                {
                    entity.Translations.Add(new ProductTranslation
                    {
                        ProductId = entity.Id,
                        Culture = t.Culture,
                        Title = t.Title,
                        Description = t.Description,
                        CreationTime = DateTime.UtcNow
                    });
                }
            }
            // Handle Images
            if (request.Images != null)
            {
                entity.Images.Clear();
                foreach (var img in request.Images)
                {
                    entity.Images.Add(new ProductImage
                    {
                        Url = img.Url,
                        IsMain = img.IsMain,
                        SortOrder = img.SortOrder
                    });
                }
            }
            // Handle Files
            if (request.Files != null)
            {
                entity.Files.Clear();
                foreach (var file in request.Files)
                {
                    entity.Files.Add(new ProductFile
                    {
                        FileUrl = file.FileUrl,
                        FileType = file.FileType
                    });
                }
            }
            // Handle Prices
            if (request.Prices != null)
            {
                entity.Prices.Clear();
                foreach (var price in request.Prices)
                {
                    entity.Prices.Add(new ProductPrice
                    {
                        Currency = price.Currency,
                        Price = price.Price,
                        StartDate = price.StartDate,
                        EndDate = price.EndDate
                    });
                }
            }
            // Handle AttributeValues
            if (request.AttributeValues != null)
            {
                entity.AttributeValues.Clear();
                foreach (var attr in request.AttributeValues)
                {
                    entity.AttributeValues.Add(new ProductAttributeValue
                    {
                        AttributeId = attr.AttributeId
                        // مقداردهی Value با setter ممکن نیست، باید بعد از ساخت شیء انجام شود
                    });
                    entity.AttributeValues.Last().SetValue(attr.Value); // متد public جدید برای مقداردهی
                }
            }
            await _dbContext.SaveChangesAsync(cancellationToken);
            return entity.Id;
        }
    }


}
