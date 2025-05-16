using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Domain.ValueObjects.Product
{
    public sealed class AttributeValue : IEquatable<AttributeValue>
    {
        public string Value { get; }

        public AttributeValue(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            Value = value;
        }

        public T As<T>()
        {
            return (T)Convert.ChangeType(Value, typeof(T));
        }

        public override bool Equals(object? obj) => Equals(obj as AttributeValue);

        public bool Equals(AttributeValue? other) => other != null && Value == other.Value;

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => Value;
    }

}
