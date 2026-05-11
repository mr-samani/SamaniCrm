using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using SamaniCrm.Core.Shared.Enums;

namespace SamaniCrm.Domain.Entities;

public class Page : BaseEntity
{
    public string? CoverImage { get; set; }
    public Guid? AuthorId { get; set; }
    public DateTime? PublishedAt { get; set; }
    public PageStatusEnum Status { get; set; }
    public PageTypeEnum Type { get; set; }



    public bool IsActive { get; set; } = true;
    public bool IsSystem { get; set; } = false;

    public virtual ICollection<PageTranslation> Translations { get; set; } = [];

}


