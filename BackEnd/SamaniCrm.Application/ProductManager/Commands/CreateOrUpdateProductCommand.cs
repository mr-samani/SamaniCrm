using MediatR;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.ProductManagerManager.Dtos;
using SamaniCrm.Domain.Entities.ProductEntities;
using SamaniCrm.Domain.ValueObjects.Product;
using System;
using System.ComponentModel.DataAnnotations;

namespace SamaniCrm.Application.ProductManagerManager.Commands
{
    public class CreateOrUpdateProductCommand : IRequest<Guid>
    {
        public Guid? Id { get; set; }
        public Guid CategoryId { get; set; }
        public Guid ProductTypeId { get; set; }
        [Required]
        [MaxLength(100)]
        public string SKU { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public string? Tags { get; set; }

        public List<ProductImageDto>? Images { get; set; } = new List<ProductImageDto>();

        public List<ProductFileDto>? Files { get; set; } = new List<ProductFileDto>();
        public List<ProductPriceDto>? Prices { get; set; } = new List<ProductPriceDto>();
        public List<ProductTranslationDto>? Translations { get; set; } = new List<ProductTranslationDto>();

        public List<ProductAttributeValueDto>? AttributeValues { get; set; } = new List<ProductAttributeValueDto>();
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
            entity.CategoryId = request.CategoryId;
            entity.ProductTypeId = request.ProductTypeId;
            entity.Slug = request.Slug;
            entity.SKU = Sku.Create(request.SKU);
            entity.IsActive = request.IsActive;
            entity.LastModifiedTime = DateTime.UtcNow;
            entity.Tags = request.Tags;

            // Handle SKU
            // اگر نیاز به تبدیل خاصی برای SKU دارید اینجا انجام دهید
            // entity.SKU = new Sku(request.SKU); // اگر Sku یک ValueObject باشد

            // Handle Translations
            if (request.Translations != null)
            {
                var toRemove = entity.Translations.Where(t => !(request.Translations.Any(rt => rt.Culture == t.Culture))).ToList();
                foreach (var t in toRemove)
                    entity.Translations.Remove(t);
                foreach (var t in request.Translations)
                {
                    var existingTranslation = entity.Translations.FirstOrDefault(x => x.Culture == t.Culture);
                    if (existingTranslation != null)
                    {
                        existingTranslation.Title = t.Title;
                        existingTranslation.Description = t.Description;
                        existingTranslation.Content = t.Content;
                    }
                    else
                    {
                        entity.Translations.Add(new ProductTranslation
                        {
                            ProductId = entity.Id,
                            Culture = t.Culture,
                            Title = t.Title,
                            Description = t.Description,
                            Content = t.Content,
                            CreationTime = DateTime.UtcNow
                        });
                    }
                }
            }
            // Handle Images
            if (request.Images != null)
            {
                var toRemove = entity.Images.Where(img => !(request.Images.Any(rimg => rimg.Url == img.Url))).ToList();
                foreach (var img in toRemove)
                    entity.Images.Remove(img);
                foreach (var img in request.Images)
                {
                    var existingImage = entity.Images.FirstOrDefault(x => x.Url == img.Url);
                    if (existingImage != null)
                    {
                        existingImage.IsMain = img.IsMain;
                        existingImage.SortOrder = img.SortOrder;
                    }
                    else
                    {
                        entity.Images.Add(new ProductImage
                        {
                            Url = img.Url,
                            IsMain = img.IsMain,
                            SortOrder = img.SortOrder
                        });
                    }
                }
            }
            // Handle Files
            if (request.Files != null)
            {
                var toRemove = entity.Files.Where(f => !(request.Files.Any(rf => rf.FileUrl == f.FileUrl))).ToList();
                foreach (var f in toRemove)
                    entity.Files.Remove(f);
                foreach (var file in request.Files)
                {
                    var existingFile = entity.Files.FirstOrDefault(x => x.FileUrl == file.FileUrl);
                    if (existingFile != null)
                    {
                        existingFile.FileType = file.FileType;
                    }
                    else
                    {
                        entity.Files.Add(new ProductFile
                        {
                            FileUrl = file.FileUrl,
                            FileType = file.FileType
                        });
                    }
                }
            }
            // Handle Prices
            if (request.Prices != null)
            {
                var toRemove = entity.Prices.Where(p => !(request.Prices.Any(rp => rp.Currency == p.Currency && rp.StartDate == p.StartDate))).ToList();
                foreach (var p in toRemove)
                    entity.Prices.Remove(p);
                foreach (var price in request.Prices)
                {
                    var existingPrice = entity.Prices.FirstOrDefault(x => x.Currency == price.Currency && x.StartDate == price.StartDate);
                    if (existingPrice != null)
                    {
                        existingPrice.Price = price.Price;
                        existingPrice.EndDate = price.EndDate;
                    }
                    else
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
            }
            // Handle AttributeValues
            if (request.AttributeValues != null)
            {
                var toRemove = entity.AttributeValues.Where(a => !(request.AttributeValues.Any(ra => ra.AttributeId == a.AttributeId))).ToList();
                foreach (var a in toRemove)
                    entity.AttributeValues.Remove(a);
                foreach (var attr in request.AttributeValues)
                {
                    var existingAttr = entity.AttributeValues.FirstOrDefault(x => x.AttributeId == attr.AttributeId);
                    if (existingAttr != null)
                    {
                        existingAttr.Value = new AttributeValue(attr.Value);
                    }
                    else
                    {
                        var newAttr = new ProductAttributeValue
                        {
                            AttributeId = attr.AttributeId
                        };
                        newAttr.Value = new AttributeValue(attr.Value);
                        entity.AttributeValues.Add(newAttr);
                    }
                }
            }
            await _dbContext.SaveChangesAsync(cancellationToken);
            return entity.Id;
        }
    }


}
