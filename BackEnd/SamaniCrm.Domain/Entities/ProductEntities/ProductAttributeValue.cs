using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamaniCrm.Domain.Interfaces;
using SamaniCrm.Domain.ValueObjects.Product;

namespace SamaniCrm.Domain.Entities.ProductEntities
{
    public class ProductAttributeValue:IAuditableEntity,ISoftDelete
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public Guid AttributeId { get; set; }

        // public string Value { get; set; } = default!;
        [MaxLength(1000)]
        public AttributeValue Value { get; private set; } = default!;

        // private AttributeValue _value;

        //public AttributeValue Value
        //{
        //    get => _value;
        //    set => _value = value ?? throw new ArgumentNullException(nameof(value));
        //}

        public virtual Product Product { get; set; } = default!;
        public virtual ProductAttribute Attribute { get; set; } = default!;



        // Implementing IAuditableEntity properties
        public DateTime CreationTime { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? LastModifiedTime { get; set; }
        public string? LastModifiedBy { get; set; }

        // Implementing ISoftDelete properties
        public bool IsDeleted { get; set; }
        public DateTime? DeletedTime { get; set; }
        public string? DeletedBy { get; set; }

        public void SetValue(string value)
        {
            // فرض بر این است که AttributeValue یک سازنده public دارد
            // و پراپرتی Value فقط getter دارد
            var field = typeof(AttributeValue).GetField("<Value>k__BackingField", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (field != null)
                field.SetValue(this.Value, value);
        }
    }

}
