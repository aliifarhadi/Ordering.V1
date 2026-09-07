using AeroTech.Ordering.Domain._Shared.Attributes;

namespace AeroTech.Ordering.Domain._Shared.Enums
{
    [SpecRef("DD-ENUM-010")]
    public enum ChargeType
    {
        [SpecRef("DD-ENUM-010")]
        Base = 1,

        [SpecRef("DD-ENUM-010")]
        Tax = 2,

        [SpecRef("DD-ENUM-010")]
        CarrierFee = 3,

        [SpecRef("DD-ENUM-010")]
        ServiceFee = 4,

        [SpecRef("DD-ENUM-010")]
        Discount = 5,

        [SpecRef("DD-ENUM-010")]
        Penalty = 6,
    }
}
