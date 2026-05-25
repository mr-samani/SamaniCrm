using SamaniCrm.Domain.ValueObjects.Product;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Domain.Entities;

public class ProductAttributeValue : BaseEntity
{
    public Guid ProductId { get; set; }
    public Guid AttributeId { get; set; }

    // public string Value { get; set; } = default!;
    [MaxLength(1000)]
    public AttributeValue Value { get; set; } = default!;

    // private AttributeValue _value;

    //public AttributeValue Value
    //{
    //    get => _value;
    //    set => _value = value ?? throw new ArgumentNullException(nameof(value));
    //}

    public virtual Product Product { get; set; } = default!;
    public virtual ProductAttribute Attribute { get; set; } = default!;



}
