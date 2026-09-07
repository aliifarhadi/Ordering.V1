using AeroTech.Ordering.Domain._Shared.Attributes;

namespace AeroTech.Ordering.Domain._Shared.Enums
{
    [SpecRef("DD-ENUM-007")]
    public enum FormOfPayment
    {
        [SpecRef("DD-ENUM-007")]
        Card = 1,

        [SpecRef("DD-ENUM-007")]
        AgencyCredit = 2,

        [SpecRef("DD-ENUM-007")]
        AgencyPrepaid = 3,

        [SpecRef("DD-ENUM-007")]
        Wallet = 4,

        [SpecRef("DD-ENUM-007")]
        Cash = 5,
    }
}
