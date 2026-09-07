using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Resources;

namespace AeroTech.Ordering.Domain._Shared.ValueObjects
{
    [SpecRef("DD-VO-011")]
    public readonly record struct CabinCode
    {
        [SpecRef("DD-VO-011")]
        public CabinCode(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw ExceptionFactory.IdentifierIsRequired(nameof(CabinCode));

            Value = value;
        }

        [SpecRef("DD-VO-011")]
        public string Value { get; }

        [SpecRef("DD-VO-011")]
        public override string ToString() => Value;
    }
}
