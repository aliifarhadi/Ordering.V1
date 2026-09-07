using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Resources;

namespace AeroTech.Ordering.Domain._Shared.ValueObjects
{
    [SpecRef("DD-VO-002")]
    public readonly record struct CurrencyCode
    {
        [SpecRef("DD-VO-002")]
        public CurrencyCode(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || value.Length != 3 || !value.All(char.IsAsciiLetterUpper))
                throw ExceptionFactory.CurrencyCodeIsInvalid(value);

            Value = value;
        }

        [SpecRef("DD-VO-002")]
        public string Value { get; }

        [SpecRef("DD-VO-002")]
        public override string ToString() => Value;
    }
}
