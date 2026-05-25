using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SamaniCrm.Core.Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Domain.Entities;

public class TenantSetting : BaseEntity
{

    public Guid TenantId { get; set; }

    [MaxLength(100)]
    public required string Category { get; set; }

    [MaxLength(100)]
    public required string Key { get; set; }

    public string? Value { get; set; } = null;

    [Description("1=String, 2=Number, 3=Boolean, 4=JSON")]
    public TenantSettingValueType ValueType { get; set; } = TenantSettingValueType.String;

    public bool IsEncrypted { get; set; } = false;

    public virtual Tenant Tenant { get; set; } = null!;


}



