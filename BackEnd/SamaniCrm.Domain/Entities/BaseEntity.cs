using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Domain.Entities;

public abstract class BaseEntity : AuditedEntity
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

}


public interface IAuditedEntity
{
    DateTime CreatedAt { get; set; }
    Guid? CreatedBy { get; set; }
    DateTime? ModifiedAt { get; set; }
    Guid? ModifiedBy { get; set; }
    DateTime? DeletedAt { get; set; }
    Guid? DeletedBy { get; set; }
    bool IsDeleted { get; set; }
    byte[]? RowVersion { get; set; }
}

public abstract class AuditedEntity : IAuditedEntity
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid? CreatedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public Guid? ModifiedBy { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }
    public bool IsDeleted { get; set; } = false;

    public byte[]? RowVersion { get; set; }
}