using AeroTech.Ordering.Domain._Shared.Attributes;

namespace AeroTech.Ordering.Domain._Shared.Enums
{
    [SpecRef("DD-ENUM-015")]
    public enum CancellationReason
    {
        [SpecRef("DD-ENUM-015")]
        CustomerRequest = 1,

        [SpecRef("DD-ENUM-015")]
        TimeLimitExpiry = 2,

        [SpecRef("DD-ENUM-015")]
        PaymentFailed = 3,

        [SpecRef("DD-ENUM-015")]
        OfferExpired = 4,

        [SpecRef("DD-ENUM-015")]
        Void = 5,

        [SpecRef("DD-ENUM-015")]
        Disruption = 6,

        [SpecRef("DD-ENUM-015")]
        Fraud = 7,

        [SpecRef("DD-ENUM-015")]
        AgentError = 8,

        [SpecRef("DD-ENUM-015")]
        ScheduleChangeRejected = 9,
    }
}
