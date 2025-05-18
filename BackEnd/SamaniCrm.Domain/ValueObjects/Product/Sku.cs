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
    public sealed class Sku : IEquatable<Sku>
    {
        public string Value { get; }

        private Sku(string value)
        {
            Value = value;
        }

        public static Sku Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("SKU cannot be empty.");

            if (!Regex.IsMatch(value, @"^[a-zA-Z0-9_-]{3,100}$"))
                throw new ArgumentException("Invalid SKU format.");

            return new Sku(value);
        }

        public override string ToString() => Value;

        // برای EF Core
        private Sku() => Value = string.Empty;

        public bool Equals(Sku? other) => other is not null && Value == other.Value;
        public override bool Equals(object? obj) => obj is Sku sku && Equals(sku);
        public override int GetHashCode() => Value.GetHashCode();
    }

}
