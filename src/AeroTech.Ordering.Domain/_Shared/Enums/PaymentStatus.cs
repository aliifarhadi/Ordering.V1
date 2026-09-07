using AeroTech.Ordering.Domain._Shared.Attributes;

namespace AeroTech.Ordering.Domain._Shared.Enums
{
    [SpecRef("DD-ENUM-006")]
    public enum PaymentStatus
    {
        [SpecRef("DD-ENUM-006")]
        Pending = 1,

        [SpecRef("DD-ENUM-006")]
        Settled = 2,

        [SpecRef("DD-ENUM-006")]
        Failed = 3,

        [SpecRef("DD-ENUM-006")]
        Reversed = 4,

        [SpecRef("DD-ENUM-006")]
        PartiallyRefunded = 5,

        [SpecRef("DD-ENUM-006")]
        Refunded = 6,
    }
}
