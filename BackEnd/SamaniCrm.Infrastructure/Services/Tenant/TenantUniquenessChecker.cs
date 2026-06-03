using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.Features.Tenants.Interfaces;
using SamaniCrm.Infrastructure.DbContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Infrastructure.Services.TenantService;


public class TenantUniquenessChecker : ITenantUniquenessChecker
{
    private readonly MasterDbContext _context;

    public TenantUniquenessChecker(MasterDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ExistsBySlugAsync(string slug, CancellationToken cancellation)
    {
        return await _context.Tenants
            .AnyAsync(t => t.Slug.ToLower() == slug.ToLower(), cancellation);
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellation)
    {
        return await _context.Tenants
            .AnyAsync(t => t.Email.ToLower() == email.ToLower(), cancellation);
    }

    public async Task<bool> ExistsByUserEmailAsync(string email, CancellationToken cancellation)
    {
        return await _context.Users
            .AnyAsync(u => u.Email != null && u.Email.ToLower() == email.ToLower(), cancellation);
    }
}