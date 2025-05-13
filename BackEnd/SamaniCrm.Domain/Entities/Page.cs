using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using SamaniCrm.Domain.Interfaces;

namespace SamaniCrm.Domain.Entities;

public class Page : IAuditableEntity, ISoftDelete
{
    public Guid Id { get; set; }
    [MaxLength(200)]
    public string? Slag { get; set; }


    public string? CoverImage { get; set; }
    public Guid? AuthorId { get; set; }
    public DateTime? PublishedAt { get; set; }
    public PageStatusEnum Status { get; set; }


    public bool IsActive { get; set; } = true;
    public bool IsSystem { get; set; } = false;

    public virtual ICollection<PageTranslation> Translations { get; set; } = [];



    // Implementing IAuditableEntity properties
    public DateTime CreationTime { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? LastModifiedTime { get; set; }
    public string? LastModifiedBy { get; set; }

    // Implementing ISoftDelete properties
    public bool IsDeleted { get; set; }
    public DateTime? DeletedTime { get; set; }
    public string? DeletedBy { get; set; }
}


public enum PageStatusEnum
{
    Draft,
    Published,
    Archived
}
