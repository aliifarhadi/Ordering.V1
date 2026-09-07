using AeroTech.Ordering.Domain._Shared.Attributes;

namespace AeroTech.Ordering.Domain._Shared.Enums
{
    [SpecRef("CDR-ENUM-ServiceDeliveryStatus")]
    public enum ServiceDeliveryStatus
    {
        [SpecRef("CDR-ENUM-ServiceDeliveryStatus")]
        Open = 1,

        [SpecRef("CDR-ENUM-ServiceDeliveryStatus")]
        Consumed = 2,

        [SpecRef("CDR-ENUM-ServiceDeliveryStatus")]
        Refunded = 3,

        [SpecRef("CDR-ENUM-ServiceDeliveryStatus")]
        Void = 4,
    }
}
