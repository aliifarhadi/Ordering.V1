using AeroTech.Ordering.Domain._Shared.Attributes;

namespace AeroTech.Ordering.Domain._Shared.Enums
{
    [SpecRef("CDR-ENUM-TimeLimitType")]
    public enum TimeLimitType
    {
        [SpecRef("CDR-ENUM-TimeLimitType")]
        Payment = 1,

        [SpecRef("CDR-ENUM-TimeLimitType")]
        HoldExpiry = 2,

        [SpecRef("CDR-ENUM-TimeLimitType")]
        NameEntry = 3,

        [SpecRef("CDR-ENUM-TimeLimitType")]
        DepositDue = 4,
    }
}
