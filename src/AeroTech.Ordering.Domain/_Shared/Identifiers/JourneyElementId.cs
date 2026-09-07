using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Resources;

namespace AeroTech.Ordering.Domain._Shared.Identifiers
{
    [SpecRef("DD-ID-003")]
    public readonly record struct JourneyElementId
    {
        [SpecRef("DD-ID-003")]
        public JourneyElementId(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw ExceptionFactory.IdentifierIsRequired(nameof(JourneyElementId));

            Value = value;
        }

        [SpecRef("DD-ID-003")]
        public string Value { get; }

        [SpecRef("DD-ID-003")]
        public override string ToString() => Value;
    }
}
