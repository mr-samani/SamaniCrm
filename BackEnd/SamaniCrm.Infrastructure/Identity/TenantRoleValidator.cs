using Microsoft.AspNetCore.Identity;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Infrastructure.DbContexts;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace SamaniCrm.Infrastructure.Identity;

public class TenantRoleValidator : RoleValidator<ApplicationRole>
{
    private readonly TenantDbContext _dbContext;

    public TenantRoleValidator(TenantDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override async Task<IdentityResult> ValidateAsync(
        RoleManager<ApplicationRole> manager,
        ApplicationRole role)
    {
        //var baseResult = await base.ValidateAsync(manager, role);

        //if (!baseResult.Succeeded)
        //    return baseResult;


        var exists = await _dbContext.Roles.AnyAsync(x =>
            x.TenantId == role.TenantId &&
            x.NormalizedName == role.NormalizedName &&
            x.Id != role.Id);

        if (exists)
        {
            return IdentityResult.Failed(
                new IdentityError
                {
                    Code = "DuplicateRoleName",
                    Description = $"Role '{role.Name}' already exists in tenant."
                });
        }

        return IdentityResult.Success;
    }
}
