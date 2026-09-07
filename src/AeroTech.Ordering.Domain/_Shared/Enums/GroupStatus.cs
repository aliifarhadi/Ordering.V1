using AeroTech.Ordering.Domain._Shared.Attributes;

namespace AeroTech.Ordering.Domain._Shared.Enums
{
    [SpecRef("CDR-ENUM-GroupStatus")]
    public enum GroupStatus
    {
        [SpecRef("CDR-ENUM-GroupStatus")]
        Draft = 1,

        [SpecRef("CDR-ENUM-GroupStatus")]
        Held = 2,

        [SpecRef("CDR-ENUM-GroupStatus")]
        PartiallyAllocated = 3,

        [SpecRef("CDR-ENUM-GroupStatus")]
        FullyAllocated = 4,

        [SpecRef("CDR-ENUM-GroupStatus")]
        Released = 5,

        [SpecRef("CDR-ENUM-GroupStatus")]
        Cancelled = 6,
    }
}
