using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SamaniCrm.Domain.Entities;
using SamaniCrm.Domain.Interfaces;
using SamaniCrm.Infrastructure.Services.TenantService;

namespace SamaniCrm.Infrastructure.DbContexts;

public static class ChangeTrackerExtensions
{
    public static void ApplyAuditInformation(this ChangeTracker changeTracker,
        Guid? tenantId,
        Guid? userId,
        bool isSeeding)
    {
        if (isSeeding)
            return;

        var entries = changeTracker
            .Entries()
            .Where(x =>
                x.State != EntityState.Detached &&
                x.State != EntityState.Unchanged)
            .ToList();

        foreach (var entry in entries)
        {
            // --- TenantId فقط برای entity های جدید ---
            if (entry.Entity is IMayHaveTenant tenantEntity)
            {
                if (entry.State == EntityState.Added)
                {
                    // فقط اگر قبلاً ست نشده باشد
                    if (tenantEntity.TenantId == default)
                    {
                        tenantEntity.TenantId = tenantId;
                    }
                }
            }

            // --- Audit Fields ---
            if (entry.Entity is IAuditedEntity auditedEntity)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        auditedEntity.CreatedAt = DateTime.UtcNow;
                        auditedEntity.CreatedBy = userId;
                        break;

                    case EntityState.Modified:
                        auditedEntity.ModifiedAt = DateTime.UtcNow;
                        auditedEntity.ModifiedBy = userId;

                        if (auditedEntity.IsDeleted)
                        {
                            auditedEntity.DeletedAt = DateTime.UtcNow;
                            auditedEntity.DeletedBy = userId;
                        }
                        break;

                    case EntityState.Deleted:
                        // تبدیل Hard Delete به Soft Delete
                        auditedEntity.IsDeleted = true;
                        auditedEntity.DeletedAt = DateTime.UtcNow;
                        auditedEntity.DeletedBy = userId;
                        entry.State = EntityState.Modified;
                        break;
                }


            }
        }
    }


}