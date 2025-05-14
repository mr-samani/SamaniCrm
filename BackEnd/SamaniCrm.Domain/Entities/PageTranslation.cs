using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using SamaniCrm.Domain.Interfaces;

namespace SamaniCrm.Domain.Entities;

public class PageTranslation : IAuditableEntity, ISoftDelete
{
    public Guid Id { get; set; }
    public Guid PageId { get; set; }

    [MaxLength(10)]
    public required string Culture { get; set; }

    [MaxLength(1000)]
    public string? Title { get; set; }
    [MaxLength(2000)]
    public string? Abstract { get; set; }
    [MaxLength(2000)]
    public string? Description { get; set; }

    [MaxLength(2000)]
    public string? MetaDescription { get; set; }
    [MaxLength(2000)]
    public string? MetaKeywords { get; set; }

    [Description("GreapsJs Data")]
    public string? Data { get; set; }
    public string? Styles { get; set; }
    public string? Scripts { get; set; }
    public string? Html { get; set; }


    public virtual Language Language { get; set; }
    public virtual Page Page { get; set; }




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

