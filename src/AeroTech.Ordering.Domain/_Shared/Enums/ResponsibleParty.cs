using AeroTech.Ordering.Domain._Shared.Attributes;

namespace AeroTech.Ordering.Domain._Shared.Enums
{
    [SpecRef("DD-ENUM-020")]
    public enum ResponsibleParty
    {
        [SpecRef("DD-ENUM-020")]
        OwnCarrier = 1,

        [SpecRef("DD-ENUM-020")]
        Partner = 2,

        [SpecRef("DD-ENUM-020")]
        Shared = 3,
    }
}
