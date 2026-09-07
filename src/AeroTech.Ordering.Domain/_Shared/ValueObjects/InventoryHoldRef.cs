using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Resources;

namespace AeroTech.Ordering.Domain._Shared.ValueObjects
{
    [SpecRef("DD-VO-021")]
    public readonly record struct InventoryHoldRef
    {
        [SpecRef("DD-VO-021")]
        public InventoryHoldRef(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw ExceptionFactory.IdentifierIsRequired(nameof(InventoryHoldRef));

            Value = value;
        }

        [SpecRef("DD-VO-021")]
        public string Value { get; }

        [SpecRef("DD-VO-021")]
        public override string ToString() => Value;
    }
}
