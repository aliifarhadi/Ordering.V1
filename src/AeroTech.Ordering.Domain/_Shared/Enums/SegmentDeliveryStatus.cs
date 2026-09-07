using AeroTech.Ordering.Domain._Shared.Attributes;

namespace AeroTech.Ordering.Domain._Shared.Enums
{
    [SpecRef("CDR-ENUM-SegmentDeliveryStatus")]
    public enum SegmentDeliveryStatus
    {
        [SpecRef("CDR-ENUM-SegmentDeliveryStatus")]
        Open = 1,

        [SpecRef("CDR-ENUM-SegmentDeliveryStatus")]
        ControlTransferred = 2,

        [SpecRef("CDR-ENUM-SegmentDeliveryStatus")]
        CheckedIn = 3,

        [SpecRef("CDR-ENUM-SegmentDeliveryStatus")]
        Boarded = 4,

        [SpecRef("CDR-ENUM-SegmentDeliveryStatus")]
        Flown = 5,

        [SpecRef("CDR-ENUM-SegmentDeliveryStatus")]
        NoShow = 6,

        [SpecRef("CDR-ENUM-SegmentDeliveryStatus")]
        Exchanged = 7,

        [SpecRef("CDR-ENUM-SegmentDeliveryStatus")]
        Refunded = 8,

        [SpecRef("CDR-ENUM-SegmentDeliveryStatus")]
        Void = 9,
    }
}
