using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamaniCrm.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace SamaniCrm.Application.Common.Interfaces;
public interface IDbContext
{
    DbSet<RefreshToken> RefreshTokens { get; set; }
    DbSet<Permission> Permissions { get; set; }
    DbSet<RolePermission> RolePermissions { get; set; }
    IQueryable<IUser> Users { get; } // فقط برای خواندن
    IQueryable<IRole> UserRoles { get; } // فقط برای خواندن

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}

