using System;

namespace SamaniCrm.Domain.ValueObjects.Product
{
    public sealed class AttributeValue : IEquatable<AttributeValue>
    {
        // برای EF Core
        private AttributeValue() => Value = string.Empty;

        // این constructor عمومی برای استفاده‌های معمولی
        public AttributeValue(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(nameof(value));

            Value = value;
        }

        // EF Core به این setter نیاز داره (می‌تونه private/internal باشه)
        public string Value { get; private set; } = string.Empty;

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
