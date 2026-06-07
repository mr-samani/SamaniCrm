using Microsoft.Extensions.Configuration;
using SamaniCrm.Core.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SamaniCrm.Infrastructure.TenantManager;

public sealed record TenantContext(
    Guid? TenantId,
    string? TenantSlug,
    string? TenantName,
    string? ConnectionString);