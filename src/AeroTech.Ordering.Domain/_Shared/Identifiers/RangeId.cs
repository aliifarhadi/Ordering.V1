using AeroTech.Ordering.Domain._Shared.Attributes;

namespace AeroTech.Ordering.Domain._Shared.Identifiers
{
    [SpecRef("CDR-ID-RangeId")]
    public readonly record struct RangeId
    {
        [SpecRef("CDR-ID-RangeId")]
        public RangeId(Guid value) => Value = value;

        [SpecRef("CDR-ID-RangeId")]
        public Guid Value { get; }

        [SpecRef("CDR-ID-RangeId")]
        public static RangeId New() => new(Guid.CreateVersion7());

        [SpecRef("CDR-ID-RangeId")]
        public override string ToString() => Value.ToString();
    }
}
