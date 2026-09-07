using AeroTech.Ordering.Domain._Shared.Attributes;

namespace AeroTech.Ordering.Domain._Shared.Enums
{
    [SpecRef("DD-ENUM-004")]
    public enum SegmentStatus
    {
        [SpecRef("DD-ENUM-004")]
        Requested = 1,

        [SpecRef("DD-ENUM-004")]
        Confirmed = 2,

        [SpecRef("DD-ENUM-004")]
        Rebooked = 3,

        [SpecRef("DD-ENUM-004")]
        Cancelled = 4,
    }
}
