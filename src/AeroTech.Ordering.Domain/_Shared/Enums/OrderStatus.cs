using AeroTech.Ordering.Domain._Shared.Attributes;

namespace AeroTech.Ordering.Domain._Shared.Enums
{
    [SpecRef("DD-ENUM-003")]
    public enum OrderStatus
    {
        [SpecRef("DD-ENUM-003")]
        Draft = 1,

        [SpecRef("DD-ENUM-003")]
        Pending = 2,

        [SpecRef("DD-ENUM-003")]
        Confirmed = 3,

        [SpecRef("DD-ENUM-003")]
        Delivered = 4,

        [SpecRef("DD-ENUM-003")]
        PartiallyFlown = 5,

        [SpecRef("DD-ENUM-003")]
        Flown = 6,

        [SpecRef("DD-ENUM-003")]
        Cancelled = 7,

        [SpecRef("DD-ENUM-003")]
        Closed = 8,
    }
}
