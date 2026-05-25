using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamaniCrm.Core.Shared.Enums;

namespace SamaniCrm.Domain.Entities;

public class Localization : BaseEntity
{
    [MaxLength(10)]
    public required string Culture { get; set; }
    [MaxLength(250)]
    public required string Key { get; set; }
    [MaxLength(2000)]
    public string? Value { get; set; }

    [MaxLength(100)]
    public LocalizationCategoryEnum Category { get; set; } = LocalizationCategoryEnum.App;


    public virtual Language? Language { get; set; }

}




