using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Domain.Entities;

public class AuditLogEntry
{
    public Guid CorrelationId { get; set; }
    public Guid? TenantId { get; set; }
    public Guid? UserId { get; set; }

    [Description("Api request address")]
    [MaxLength(500)]
    public string? Action { get; set; }

    [MaxLength(250)]
    public string? EntityType { get; set; }

    [MaxLength(20)]
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public int? StatusCode { get; set; }
    public int? DurationMs { get; set; }
    public DateTime Timestamp { get; set; }
}
