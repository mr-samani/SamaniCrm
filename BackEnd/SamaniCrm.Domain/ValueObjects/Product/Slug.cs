using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SamaniCrm.Domain.ValueObjects.Product
{
    public sealed class Slug : IEquatable<Slug>
    {
        public string Value { get; }

        public Slug(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Slug نمی‌تواند خالی باشد.", nameof(value));

            // اعتبارسنجی: فقط حروف کوچک، عدد، خط فاصله و آندرلاین
            if (!Regex.IsMatch(value, @"^[a-z0-9-_]+$"))
                throw new ArgumentException("Slug فقط می‌تواند شامل حروف کوچک، عدد، خط فاصله و آندرلاین باشد.", nameof(value));

            Value = value;
        }

        public override bool Equals(object? obj) => Equals(obj as Slug);

        public bool Equals(Slug? other) => other != null && Value == other.Value;

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => Value;
    }

}
