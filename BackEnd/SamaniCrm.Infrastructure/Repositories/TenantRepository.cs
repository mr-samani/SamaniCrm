using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.DTOs;
using SamaniCrm.Application.Features.Tenants.Dtos;
using SamaniCrm.Application.Features.Tenants.Queries;
using SamaniCrm.Core.Shared.Enums;
using SamaniCrm.Core.Shared.Interfaces.Tenant;
using SamaniCrm.Domain.Entities;

namespace SamaniCrm.Infrastructure.Repositories;

public class TenantRepository : ITenantRepository
{
    private readonly ApplicationDbContext _context;

    public TenantRepository(ApplicationDbContext context) => _context = context;

    public async Task<TenantDto?> GetByIdAsync(Guid id, CancellationToken cancellation) =>
        await _context.Tenants.Select(s => new TenantDto()
        {
            Id = s.Id,
            Logo = s.LogoUrl,
            Name = s.Name,
            Slug = s.Slug,
            Status = s.Status,
            PrimaryColor = s.PrimaryColor,
            CreatedAt = s.CreatedAt
        }).FirstOrDefaultAsync(t => t.Id == id,cancellation);

    public async Task<TenantDto?> GetBySlugAsync(string slug, CancellationToken cancellation) =>
        await _context.Tenants.Select(s => new TenantDto()
        {
            Id = s.Id,
            Logo = s.LogoUrl,
            Name = s.Name,
            Slug = s.Slug,
            Status = s.Status,
            PrimaryColor = s.PrimaryColor,
            CreatedAt = s.CreatedAt
        }).FirstOrDefaultAsync(t => t.Slug == slug,cancellation);

    public async Task<bool> SlugExistsAsync(string slug, CancellationToken cancellation, Guid? excludeId = null)
    {
        var query = _context.Tenants.Where(t => t.Slug == slug.ToLowerInvariant());
        if (excludeId.HasValue)
            query = query.Where(t => t.Id != excludeId.Value);
        return await query.AnyAsync(cancellation);
    }

    public async Task<PaginatedResult<TenantListDto>> GetAllAsync(TenantListQuery query, CancellationToken cancellation)
    {
        var q = _context.Tenants
            .Select(s => new TenantListDto()
            {
                Id = s.Id,
                Slug = s.Slug,
                Name = s.Name,
                Status = s.Status,
                UserCount = 0,
                CreatedAt = s.CreatedAt
            });
        //.Where(t => !t.IsDeleted);

        if (!string.IsNullOrWhiteSpace(query.Search))
            q = q.Where(t => t.Name.Contains(query.Search) || t.Slug.Contains(query.Search));

        if (!string.IsNullOrWhiteSpace(query.Status) && Enum.TryParse<TenantStatus>(query.Status, true, out var status))
            q = q.Where(t => t.Status == status);

        var total = await q.CountAsync(cancellation);

        q = query.SortBy?.ToLower() switch
        {
            "name" => query.SortDirection == "asc" ? q.OrderBy(t => t.Name) : q.OrderByDescending(t => t.Name),
            "status" => query.SortDirection == "asc" ? q.OrderBy(t => t.Status) : q.OrderByDescending(t => t.Status),
            _ => query.SortDirection == "asc" ? q.OrderBy(t => t.CreatedAt) : q.OrderByDescending(t => t.CreatedAt)
        };

        var items = await q.Skip((query.PageNumber - 1) * query.PageSize).Take(query.PageSize).ToListAsync(cancellation);
        return new PaginatedResult<TenantListDto>(items, total, query.PageNumber, query.PageSize);
    }


    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellation)
    {
        //var result = await _context.Tenants
        //    .Where(x => x.Id == id)
        //    .ExecuteDeleteAsync(cancellation);

        var tenant = await _context.Tenants.FindAsync(id, cancellation);
        if (tenant == null)
        {
            return false;
        }
        tenant.Status = TenantStatus.Deleted;
        tenant.IsDeleted = true;
        tenant.DeletedAt = DateTime.UtcNow;
        tenant.ModifiedAt = DateTime.UtcNow;

        var result= await _context.SaveChangesAsync(cancellation);

        return result > 0;
    }

}
