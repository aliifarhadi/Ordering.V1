using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Resources;

namespace AeroTech.Ordering.Domain._Shared.Identifiers
{
    [SpecRef("DD-ID-009")]
    public readonly record struct SellerId
    {
        [SpecRef("DD-ID-009")]
        public SellerId(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw ExceptionFactory.IdentifierIsRequired(nameof(SellerId));

            Value = value;
        }

        [SpecRef("DD-ID-009")]
        public string Value { get; }

        [SpecRef("DD-ID-009")]
        public override string ToString() => Value;
    }
}
