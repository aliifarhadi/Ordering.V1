using AeroTech.Ordering.Domain._Shared.Attributes;

namespace AeroTech.Ordering.Domain._Shared.Enums
{
    [SpecRef("DD-ENUM-002")]
    public enum OrderItemStatus
    {
        [SpecRef("DD-ENUM-002")]
        Offered = 1,

        [SpecRef("DD-ENUM-002")]
        PendingPayment = 2,

        [SpecRef("DD-ENUM-002")]
        Confirmed = 3,

        [SpecRef("DD-ENUM-002")]
        Delivered = 4,

        [SpecRef("DD-ENUM-002")]
        PartiallyUsed = 5,

        [SpecRef("DD-ENUM-002")]
        Used = 6,

        [SpecRef("DD-ENUM-002")]
        Exchanged = 7,

        [SpecRef("DD-ENUM-002")]
        Cancelled = 8,

        [SpecRef("DD-ENUM-002")]
        Refunded = 9,
    }
}
