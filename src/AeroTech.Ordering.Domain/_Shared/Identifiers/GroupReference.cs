using AeroTech.Ordering.Domain._Shared.Attributes;

namespace AeroTech.Ordering.Domain._Shared.Identifiers
{
    [SpecRef("CDR-ID-GroupReference")]
    [Blocked("BL-003", "GroupReference format, length, and alphabet", "O22")]
    public readonly record struct GroupReference
    {
        [SpecRef("CDR-ID-GroupReference")]
        public GroupReference(string value) => Value = value;

        [SpecRef("CDR-ID-GroupReference")]
        public string Value { get; }

        [SpecRef("CDR-ID-GroupReference")]
        public override string ToString() => Value;
    }
}
