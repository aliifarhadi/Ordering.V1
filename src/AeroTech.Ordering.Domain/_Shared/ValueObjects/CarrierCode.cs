using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Resources;

namespace AeroTech.Ordering.Domain._Shared.ValueObjects
{
    [SpecRef("DD-VO-008")]
    public readonly record struct CarrierCode
    {
        [SpecRef("DD-VO-008")]
        public CarrierCode(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw ExceptionFactory.IdentifierIsRequired(nameof(CarrierCode));

            Value = value;
        }

        [SpecRef("DD-VO-008")]
        public string Value { get; }

        [SpecRef("DD-VO-008")]
        public override string ToString() => Value;
    }
}
