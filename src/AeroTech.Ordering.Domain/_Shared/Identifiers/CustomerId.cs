using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Resources;

namespace AeroTech.Ordering.Domain._Shared.Identifiers
{
    [SpecRef("DD-ID-008")]
    public readonly record struct CustomerId
    {
        [SpecRef("DD-ID-008")]
        public CustomerId(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw ExceptionFactory.IdentifierIsRequired(nameof(CustomerId));

            Value = value;
        }

        [SpecRef("DD-ID-008")]
        public string Value { get; }

        [SpecRef("DD-ID-008")]
        public override string ToString() => Value;
    }
}
