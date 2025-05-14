using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamaniCrm.Application.Pages.Commands;
using SamaniCrm.Domain.Entities;

namespace SamaniCrm.Application.DTOs;
public class PageForEditDto
{
    public Guid? Id { get; set; }
    public string? CoverImage { get; set; }
    public PageStatusEnum? Status { get; set; } = PageStatusEnum.Draft;
    public PageTypeEnum Type { get; set; } = PageTypeEnum.OtherPages;

    public bool IsActive { get; set; }
    public bool IsSystem { get; set; }

    public List<PageMetaDataDto> Translations { get; set; } = [];
}
public class PageMetaDataDto
{
    public Guid? Id { get; set; }
    [MaxLength(10)]
    public required string Culture { get; set; }

    [MaxLength(1000)]
    public required string Title { get; set; }
    [MaxLength(2000)]
    public string? Abstract { get; set; }
    [MaxLength(2000)]
    public string? Description { get; set; }
    [MaxLength(2000)]
    public string? MetaDescription { get; set; }
    [MaxLength(2000)]
    public string? MetaKeywords { get; set; }

}

