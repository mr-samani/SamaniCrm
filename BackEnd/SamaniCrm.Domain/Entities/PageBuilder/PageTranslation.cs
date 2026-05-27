using SamaniCrm.Domain.Interfaces;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SamaniCrm.Domain.Entities;

public class PageTranslation : BaseTranslation,IMayHaveTenant
{
    public Guid? TenantId { get; set; }
    public Guid PageId { get; set; } 

    [MaxLength(1000)]
    public string? Title { get; set; }
    [MaxLength(2000)]
    public string? Introduction { get; set; }
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

     
    public virtual Page Page { get; set; } = default!;



}

