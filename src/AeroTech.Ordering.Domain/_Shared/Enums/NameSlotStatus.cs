using AeroTech.Ordering.Domain._Shared.Attributes;

namespace AeroTech.Ordering.Domain._Shared.Enums
{
    [SpecRef("CDR-ENUM-NameSlotStatus")]
    public enum NameSlotStatus
    {
        [SpecRef("CDR-ENUM-NameSlotStatus")]
        Unallocated = 1,

        [SpecRef("CDR-ENUM-NameSlotStatus")]
        Allocated = 2,

        [SpecRef("CDR-ENUM-NameSlotStatus")]
        Released = 3,
    }
}
