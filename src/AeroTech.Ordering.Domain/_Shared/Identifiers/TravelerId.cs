using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Resources;

namespace AeroTech.Ordering.Domain._Shared.Identifiers
{
    [SpecRef("DD-ID-002")]
    public readonly record struct TravelerId
    {
        [SpecRef("DD-ID-002")]
        public TravelerId(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw ExceptionFactory.IdentifierIsRequired(nameof(TravelerId));

            Value = value;
        }

        [SpecRef("DD-ID-002")]
        public string Value { get; }

        [SpecRef("DD-ID-002")]
        public override string ToString() => Value;
    }
}
