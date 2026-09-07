using AeroTech.Ordering.Domain._Shared.Attributes;

namespace AeroTech.Ordering.Domain._Shared.Identifiers
{
    [SpecRef("DD-ID-007")]
    public readonly record struct OperationId
    {
        [SpecRef("DD-ID-007")]
        public OperationId(Guid value) => Value = value;

        [SpecRef("DD-ID-007")]
        public Guid Value { get; }

        [SpecRef("DD-ID-007")]
        public static OperationId New() => new(Guid.CreateVersion7());

        [SpecRef("DD-ID-007")]
        public override string ToString() => Value.ToString();
    }
}
