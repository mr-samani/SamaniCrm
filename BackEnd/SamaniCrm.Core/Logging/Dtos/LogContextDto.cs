using System;
using System.Collections.Generic;
using System.Text;

namespace SamaniCrm.Core.Shared.Logging.Dtos;

public class LogContextDto
{
    public Guid? TenantId { get; set; }
    public Guid? UserId { get; set; }
    public string? UserName { get; set; }
    public string? IpAddress { get; set; }
    public string? CorrelationId { get; set; }
    public string? Source { get; set; }
    public string? ActionName { get; set; }
    public string? ControllerName { get; set; }
    public string? HttpMethod { get; set; }
    public string? RequestPath { get; set; }
    public Dictionary<string, object>? ExtraData { get; set; }
}
