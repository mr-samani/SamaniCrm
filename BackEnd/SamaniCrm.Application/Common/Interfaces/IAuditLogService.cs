using SamaniCrm.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.Common.Interfaces;

public interface IAuditLogService
{
    Task LogAsync(AuditLogEntry auditLogEntry);
}

