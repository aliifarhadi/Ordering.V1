using AeroTech.Ordering.Domain._Shared.Attributes;

namespace AeroTech.Ordering.Domain._Shared.Enums
{
    [SpecRef("CDR-ENUM-DeliveryRecordStatus")]
    public enum DeliveryRecordStatus
    {
        [SpecRef("CDR-ENUM-DeliveryRecordStatus")]
        Issued = 1,

        [SpecRef("CDR-ENUM-DeliveryRecordStatus")]
        PartiallyUsed = 2,

        [SpecRef("CDR-ENUM-DeliveryRecordStatus")]
        Used = 3,

        [SpecRef("CDR-ENUM-DeliveryRecordStatus")]
        Exchanged = 4,

        [SpecRef("CDR-ENUM-DeliveryRecordStatus")]
        Refunded = 5,

        [SpecRef("CDR-ENUM-DeliveryRecordStatus")]
        Void = 6,

        [SpecRef("CDR-ENUM-DeliveryRecordStatus")]
        Closed = 7,
    }
}
