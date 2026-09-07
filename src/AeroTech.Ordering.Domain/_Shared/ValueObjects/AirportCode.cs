using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Resources;

namespace AeroTech.Ordering.Domain._Shared.ValueObjects
{
    [SpecRef("DD-VO-010")]
    public readonly record struct AirportCode
    {
        [SpecRef("DD-VO-010")]
        public AirportCode(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw ExceptionFactory.IdentifierIsRequired(nameof(AirportCode));

            Value = value;
        }

        [SpecRef("DD-VO-010")]
        public string Value { get; }

        [SpecRef("DD-VO-010")]
        public override string ToString() => Value;
    }
}
