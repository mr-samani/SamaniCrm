using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Domain.Entities;

public class TenantDatabaseConnection : BaseEntity
{
    public Guid TenantId { get; set; }
    public DatabaseType DatabaseType { get; set; } = DatabaseType.SQLServer;

    [MaxLength(256)]
    public required string ServerName { get; set; }

    [MaxLength(128)]
    public required string DatabaseName { get; set; }

    [MaxLength(100)]
    public required string Username { get; set; }

    [MaxLength(500)]
    public required string EncryptedPassword { get; set; }

    public required string ConnectionString { get; set; }

    public bool IsMaster { get; set; } = false;
    public bool IsActive { get; set; } = true;

    public DateTime? LastHealthCheck { get; set; }

    public required HealthStatus HealthStatus { get; set; } = HealthStatus.Healthy;


    public virtual Tenant? Tenant { get; set; } = default!;


}

public enum DatabaseType
{
    SQLServer = 1,
    PostgreSQL,
    MySQL
}

public enum HealthStatus
{
    Healthy = 1,
    Degraded,
    Unhealthy
}



