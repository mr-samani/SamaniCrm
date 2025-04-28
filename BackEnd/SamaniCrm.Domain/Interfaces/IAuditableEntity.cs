using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Domain.Interfaces
{
    public interface IAuditableEntity
    {
        DateTime CreationTime { get; set; }
        string? CreatedBy { get; set; }
        DateTime? LastModifiedTime { get; set; }
        string? LastModifiedBy { get; set; }
    }
}
