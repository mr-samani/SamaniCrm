using SamaniCrm.Core.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.Features.Tenants.Dtos;

public class TenantDto
{

    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Logo { get; set; }
    public string? PrimaryColor { get; set; }
    public TenantStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? SuspendedAt { get; set; }
}
