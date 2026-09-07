using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Resources;

namespace AeroTech.Ordering.Domain._Shared.Identifiers
{
    [SpecRef("DD-ID-010")]
    public readonly record struct OfferId
    {
        [SpecRef("DD-ID-010")]
        public OfferId(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw ExceptionFactory.IdentifierIsRequired(nameof(OfferId));

            Value = value;
        }

        [SpecRef("DD-ID-010")]
        public string Value { get; }

        [SpecRef("DD-ID-010")]
        public override string ToString() => Value;
    }
}
