using MediatR;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Domain.Entities.ProductEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.ProductManagerManager.Commands
{
    public record DeleteProductCategoryCommand(Guid Id) : IRequest<bool>;


    public class DeleteProductCategoryCommandHandler : IRequestHandler<DeleteProductCategoryCommand, bool>
    {
        private readonly IApplicationDbContext _dbContext;

        public DeleteProductCategoryCommandHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Handle(DeleteProductCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _dbContext.ProductCategories.FindAsync(request.Id);
            if (category == null)
                throw new NotFoundException("ProductCategory not found.");

            var allCategories = await _dbContext.ProductCategories
                .Where(x => !x.IsDeleted)
                .ToListAsync(cancellationToken);

            var toDelete = GetAllDescendants(allCategories, request.Id);
            toDelete.Add(category);

            var categoryIds = toDelete.Select(x => x.Id).ToList();

            // ترجمه‌های همه کتگوری‌های حذف‌شونده رو یکجا بگیر
            var translations = await _dbContext.ProductCategoryTranslations
                .Where(x => categoryIds.Contains(x.CategoryId) && !x.IsDeleted)
                .ToListAsync(cancellationToken);

            var now = DateTime.UtcNow;

            foreach (var item in toDelete)
            {
                item.IsDeleted = true;
                item.DeletedTime = now;
            }

            foreach (var translation in translations)
            {
                translation.IsDeleted = true;
                translation.DeletedTime = now;
            }

            var result = await _dbContext.SaveChangesAsync(cancellationToken);
            return result > 0;
        }

        private List<ProductCategory> GetAllDescendants(
            List<ProductCategory> all,
            Guid parentId)
        {
            var result = new List<ProductCategory>();
            var children = all.Where(x => x.ParentId == parentId).ToList();

            foreach (var child in children)
            {
                result.Add(child);
                result.AddRange(GetAllDescendants(all, child.Id));
            }

            return result;
        }
    }
}
