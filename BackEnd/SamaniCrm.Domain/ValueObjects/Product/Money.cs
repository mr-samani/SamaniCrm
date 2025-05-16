using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Domain.ValueObjects.Product
{
    public sealed class Money : IEquatable<Money>
    {
        public decimal Amount { get; }
        public string Currency { get; }

        public Money(decimal amount, string currency)
        {
            if (amount < 0)
                throw new ArgumentException("مقدار پول نمی‌تواند منفی باشد.", nameof(amount));

            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("ارز باید مشخص شود.", nameof(currency));

            Amount = amount;
            Currency = currency.ToUpperInvariant();
        }

        public override bool Equals(object? obj) => Equals(obj as Money);

        public bool Equals(Money? other)
        {
            if (other == null)
                return false;

            return Amount == other.Amount && Currency == other.Currency;
        }

        public override int GetHashCode() => HashCode.Combine(Amount, Currency);

        public override string ToString() => $"{Amount} {Currency}";

        public Money Add(Money other)
        {
            if (Currency != other.Currency)
                throw new InvalidOperationException("نمی‌توان پول با ارزهای مختلف جمع کرد.");

            return new Money(Amount + other.Amount, Currency);
        }
    }

}
