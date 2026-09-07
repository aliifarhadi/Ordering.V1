using AeroTech.Ordering.Domain._Shared.Attributes;

namespace AeroTech.Ordering.Domain._Shared.Identifiers
{
    [SpecRef("DD-ID-012")]
    [Blocked("BL-003", "OrderReference format, length, and alphabet", "O22")]
    public readonly record struct OrderReference
    {
        [SpecRef("DD-ID-012")]
        public OrderReference(string value) => Value = value;

        [SpecRef("DD-ID-012")]
        public string Value { get; }

        [SpecRef("DD-ID-012")]
        public override string ToString() => Value;
    }
}
