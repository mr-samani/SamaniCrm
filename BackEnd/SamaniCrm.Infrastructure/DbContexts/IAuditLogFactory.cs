using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SamaniCrm.Domain.Attributes;
using SamaniCrm.Domain.Entities;
using SamaniCrm.Infrastructure.Services.TenantService;
using System.Security.AccessControl;
using System.Text.Json;

namespace SamaniCrm.Infrastructure.DbContexts;

public interface IAuditLogFactory
{
    List<AuditLog> Create(
        ChangeTracker tracker,
        AuditContext context);
}
public sealed class AuditContext
{
    public Guid? UserId { get; init; }

    public Guid? DelegatorId { get; init; }

    public Guid? TenantId { get; init; }

    public bool IsDelegated { get; init; }

    public string CorrelationId { get; init; } = "";
}
public class AuditLogFactory
    : IAuditLogFactory
{
    public List<AuditLog> Create(
        ChangeTracker tracker,
        AuditContext context)
    {

        var auditLogs = new List<AuditLog>();

        var entries = tracker
            .Entries()
            .Where(e =>
                e.Entity is not AuditLog &&
                e.Entity is not AppLogEntry &&
                e.State != EntityState.Detached &&
                e.State != EntityState.Unchanged)
            .ToList();

        foreach (var entry in entries)
        {
            var entityType = entry.Metadata.ClrType;

            if (Attribute.IsDefined(entityType, typeof(AuditIgnoreAttribute)))
            {
                continue;
            }
            var auditLog = new AuditLog
            {
                CorrelationId = context.CorrelationId,
                UserId = context.UserId,
                DelegatorId = context.DelegatorId,
                IsDelegated = context.IsDelegated,
                TenantId = context.TenantId,

                EntityName = entityType.Name,

                CreationTime = DateTime.UtcNow
            };

            var key = entry.Metadata.FindPrimaryKey();

            if (key != null)
            {
                auditLog.EntityId = string.Join(
                    ",",
                    key.Properties.Select(
                        p => entry.Property(p.Name).CurrentValue?.ToString()));
            }

            switch (entry.State)
            {
                case EntityState.Added:

                    auditLog.Action = "Create";

                    var createdValues = entry.Properties
                        .Where(p =>
                            !p.Metadata.IsPrimaryKey())
                        .ToDictionary(
                            p => p.Metadata.Name,
                            p => new
                            {
                                New = p.CurrentValue
                            });

                    auditLog.Changes =
                        JsonSerializer.Serialize(createdValues);

                    break;

                case EntityState.Modified:

                    auditLog.Action = "Update";

                    var modifiedValues =
                        new Dictionary<string, object>();

                    foreach (var property in entry.Properties)
                    {

                        if (!property.IsModified)
                            continue;

                        if (property.Metadata.PropertyInfo?
                          .IsDefined(typeof(AuditIgnoreAttribute), true) == true)
                        {
                            continue;
                        }

                        if (Equals(
                            property.OriginalValue,
                            property.CurrentValue))
                            continue;

                        modifiedValues[property.Metadata.Name] =
                            new
                            {
                                Old = property.OriginalValue,
                                New = property.CurrentValue
                            };
                    }

                    if (modifiedValues.Count == 0)
                        continue;

                    auditLog.Changes =
                        JsonSerializer.Serialize(modifiedValues);

                    break;

                case EntityState.Deleted:

                    auditLog.Action = "Delete";

                    var deletedValues = entry.Properties
                        .Where(p =>
                            !p.Metadata.IsPrimaryKey())
                        .ToDictionary(
                            p => p.Metadata.Name,
                            p => new
                            {
                                Old = p.OriginalValue
                            });

                    auditLog.Changes =
                        JsonSerializer.Serialize(deletedValues);

                    break;
            }

            auditLogs.Add(auditLog);
        }

        return auditLogs;
    }
}