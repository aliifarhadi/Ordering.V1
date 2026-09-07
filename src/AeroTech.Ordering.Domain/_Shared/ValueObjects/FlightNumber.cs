using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Resources;

namespace AeroTech.Ordering.Domain._Shared.ValueObjects
{
    [SpecRef("DD-VO-009")]
    public readonly record struct FlightNumber
    {
        [SpecRef("DD-VO-009")]
        public FlightNumber(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw ExceptionFactory.IdentifierIsRequired(nameof(FlightNumber));

            Value = value;
        }

        [SpecRef("DD-VO-009")]
        public string Value { get; }

        [SpecRef("DD-VO-009")]
        public override string ToString() => Value;
    }
}
