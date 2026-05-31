using System;
using System.Collections.Generic;
using System.Text;

namespace SamaniCrm.Domain.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public sealed class AuditIgnoreAttribute : Attribute
{
}




    //[AuditIgnore]
    //public string HtmlContent { get; set; }

    //[AuditIgnore]
    //public byte[] FileContent { get; set; }