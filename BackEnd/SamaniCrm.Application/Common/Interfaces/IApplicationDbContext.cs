using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Domain.Entities;
using MenuEntity = SamaniCrm.Domain.Entities.Menu;
namespace SamaniCrm.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<Localization> Localizations { get; set; }
        public DbSet<MenuEntity> Menus { get; set; }
        public DbSet<MenuTranslation> MenuTranslations { get; set; }


        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        int SaveChanges();
    }
}
