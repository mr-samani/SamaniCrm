using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SamaniCrm.Domain.ValueObjects.Product
{
    [ComplexType]
    public sealed class Sku : IEquatable<Sku>
    {
        public string Value { get; }

        public Sku(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("SKU نمی تواند خالی باشد.", nameof(value));

            // نمونه اعتبارسنجی ساده: فقط حروف، عدد و خط تیره مجاز است
            if (!Regex.IsMatch(value, @"^[a-zA-Z0-9\-]+$"))
                throw new ArgumentException("SKU فقط می تواند شامل حروف، عدد و خط تیره باشد.", nameof(value));

            Value = value.ToUpperInvariant();
        }

        public override bool Equals(object? obj) => Equals(obj as Sku);

        public bool Equals(Sku? other) => other != null && Value == other.Value;

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => Value;
    }

}
