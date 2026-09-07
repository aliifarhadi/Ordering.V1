using AeroTech.Ordering.Domain._Shared.Attributes;

namespace AeroTech.Ordering.Domain._Shared.Enums
{
    [SpecRef("DD-ENUM-005")]
    public enum DeliveryStatus
    {
        [SpecRef("DD-ENUM-005")]
        NotStarted = 1,

        [SpecRef("DD-ENUM-005")]
        CheckedIn = 2,

        [SpecRef("DD-ENUM-005")]
        Boarded = 3,

        [SpecRef("DD-ENUM-005")]
        Flown = 4,

        [SpecRef("DD-ENUM-005")]
        NoShow = 5,
    }
}
