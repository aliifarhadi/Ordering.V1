using AeroTech.Ordering.Domain._Shared.Attributes;

namespace AeroTech.Ordering.Domain._Shared.Identifiers
{
    [SpecRef("CDR-ID-GroupBookingId")]
    public readonly record struct GroupBookingId
    {
        [SpecRef("CDR-ID-GroupBookingId")]
        public GroupBookingId(Guid value) => Value = value;

        [SpecRef("CDR-ID-GroupBookingId")]
        public Guid Value { get; }

        [SpecRef("CDR-ID-GroupBookingId")]
        public static GroupBookingId New() => new(Guid.CreateVersion7());

        [SpecRef("CDR-ID-GroupBookingId")]
        public override string ToString() => Value.ToString();
    }
}
