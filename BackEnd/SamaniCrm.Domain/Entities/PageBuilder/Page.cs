using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using SamaniCrm.Core.Shared.Enums;
using SamaniCrm.Domain.Interfaces;

namespace SamaniCrm.Domain.Entities;

public class Page : BaseEntity,IMayHaveTenant
{
    [MaxLength(255)]
    public required string Slug { get; set; }

    public Guid? TenantId { get; set; }
    public string? CoverImage { get; set; }
    public Guid? AuthorId { get; set; }
    public DateTime? PublishedAt { get; set; }
    public PageStatusEnum Status { get; set; }
    public PageTypeEnum Type { get; set; }



    public bool IsActive { get; set; } = true;
    public bool IsSystem { get; set; } = false;

    public virtual ICollection<PageTranslation> Translations { get; set; } = [];

}


