using AeroTech.Ordering.Domain._Shared.Attributes;

namespace AeroTech.Ordering.Domain._Shared.Identifiers
{
    [SpecRef("DD-ID-001")]
    public readonly record struct OrderId
    {
        [SpecRef("DD-ID-001")]
        public OrderId(Guid value) => Value = value;

        [SpecRef("DD-ID-001")]
        public Guid Value { get; }

        [SpecRef("DD-ID-001")]
        public static OrderId New() => new(Guid.CreateVersion7());

        [SpecRef("DD-ID-001")]
        public override string ToString() => Value.ToString();
    }
}
