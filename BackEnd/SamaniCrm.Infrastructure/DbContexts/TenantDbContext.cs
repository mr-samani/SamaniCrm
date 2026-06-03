using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.Features.Tenants.Interfaces;
using SamaniCrm.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SamaniCrm.Infrastructure.DbContexts;

public class TenantDbContext : BaseDbContext, IApplicationDbContext
{
    public TenantDbContext(
     DbContextOptions<TenantDbContext> options,
     ICurrentUserService? currentUserService,
     ICurrentTenant? currentTenant,
     IHttpContextAccessor? httpContextAccessor) : base(options, currentUserService, currentTenant, httpContextAccessor)
    {
    }

    public DbSet<TenantSetting> TenantSettings { get; set; }


}
