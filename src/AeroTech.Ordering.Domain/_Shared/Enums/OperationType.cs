using AeroTech.Ordering.Domain._Shared.Attributes;

namespace AeroTech.Ordering.Domain._Shared.Enums
{
    [SpecRef("CDR-ENUM-OperationType")]
    public enum OperationType
    {
        [SpecRef("CDR-ENUM-OperationType")]
        Booking = 1,

        [SpecRef("CDR-ENUM-OperationType")]
        PaymentCompletion = 2,

        [SpecRef("CDR-ENUM-OperationType")]
        Delivery = 3,

        [SpecRef("CDR-ENUM-OperationType")]
        Cancellation = 4,

        [SpecRef("CDR-ENUM-OperationType")]
        Void = 5,

        [SpecRef("CDR-ENUM-OperationType")]
        Refund = 6,

        [SpecRef("CDR-ENUM-OperationType")]
        GroupNameAllocation = 7,

        [SpecRef("CDR-ENUM-OperationType")]
        TaxSubmission = 8,
    }
}
