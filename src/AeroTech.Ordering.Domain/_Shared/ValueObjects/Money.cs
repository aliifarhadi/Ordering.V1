using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Contracts;
using AeroTech.Ordering.Domain._Shared.Resources;

namespace AeroTech.Ordering.Domain._Shared.ValueObjects
{
    [SpecRef("DD-VO-001")]
    public sealed record Money
    {
        [SpecRef("DD-VO-001")]
        public Money(decimal amount, CurrencyCode currency)
        {
            Amount = amount;
            Currency = currency;
        }

        private Money()
        {
            Amount = decimal.Zero;
            Currency = default;
        }

        [SpecRef("DD-VO-001-01")]
        public decimal Amount { get; }

        [SpecRef("DD-VO-001-02")]
        public CurrencyCode Currency { get; }

        [SpecRef("DD-VO-001")]
        public static Money Zero(CurrencyCode currency) => new(decimal.Zero, currency);

        [SpecRef("DD-VO-001-01")]
        [Blocked("BL-004", "Currency scale and increment", "O2")]
        public static Money Validated(decimal amount, CurrencyCode currency, ICurrencyDefinitionProvider currencies)
        {
            if (ScaleOf(amount) > currencies.GetScale(currency))
                throw ExceptionFactory.AmountDoesNotConformToCurrencyScale(amount, currency);

            var increment = currencies.GetIncrement(currency);
            if (increment != decimal.Zero && decimal.Remainder(amount, increment) != decimal.Zero)
                throw ExceptionFactory.AmountDoesNotConformToCurrencyIncrement(amount, currency);

            return new Money(amount, currency);
        }

        [SpecRef("DD-VO-001")]
        public bool IsNegative => Amount < decimal.Zero;

        [SpecRef("DD-VO-001")]
        public bool IsZero => Amount == decimal.Zero;

        [SpecRef("DD-VO-001")]
        public static Money operator +(Money left, Money right) =>
            left.Currency == right.Currency
                ? new Money(left.Amount + right.Amount, left.Currency)
                : throw ExceptionFactory.CurrencyMismatch(left.Currency, right.Currency);

        [SpecRef("DD-VO-001")]
        public static Money operator -(Money left, Money right) =>
            left.Currency == right.Currency
                ? new Money(left.Amount - right.Amount, left.Currency)
                : throw ExceptionFactory.CurrencyMismatch(left.Currency, right.Currency);

        [SpecRef("DD-VO-001")]
        public static Money operator -(Money value) => new(-value.Amount, value.Currency);

        [SpecRef("DD-VO-001")]
        public static bool operator >(Money left, Money right) => Compare(left, right) > 0;

        [SpecRef("DD-VO-001")]
        public static bool operator <(Money left, Money right) => Compare(left, right) < 0;

        [SpecRef("DD-VO-001")]
        public static bool operator >=(Money left, Money right) => Compare(left, right) >= 0;

        [SpecRef("DD-VO-001")]
        public static bool operator <=(Money left, Money right) => Compare(left, right) <= 0;

        [SpecRef("DD-VO-001")]
        public override string ToString() => $"{Amount} {Currency}";

        private static int ScaleOf(decimal value) => (decimal.GetBits(value)[3] >> 16) & 0xFF;

        private static int Compare(Money left, Money right) =>
            left.Currency == right.Currency
                ? decimal.Compare(left.Amount, right.Amount)
                : throw ExceptionFactory.CurrencyMismatch(left.Currency, right.Currency);
    }
}
