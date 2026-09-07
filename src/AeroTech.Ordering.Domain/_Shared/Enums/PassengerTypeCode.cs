using AeroTech.Ordering.Domain._Shared.Attributes;

namespace AeroTech.Ordering.Domain._Shared.Enums
{
    [SpecRef("DD-ENUM-009")]
    public enum PassengerTypeCode
    {
        [SpecRef("DD-ENUM-009")]
        ADT = 1,

        [SpecRef("DD-ENUM-009")]
        CHD = 2,

        [SpecRef("DD-ENUM-009")]
        INF = 3,
    }
}
