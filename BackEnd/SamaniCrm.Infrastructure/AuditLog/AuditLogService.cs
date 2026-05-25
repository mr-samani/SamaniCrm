using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Infrastructure.AuditLog;

public class AuditLogService: IAuditLogService
{
    public async Task LogAsync(AuditLogEntry auditLogEntry)
    {
        throw new NotImplementedException();
    }
}
