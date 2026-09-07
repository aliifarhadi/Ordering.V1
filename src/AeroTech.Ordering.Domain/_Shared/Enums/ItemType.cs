using AeroTech.Ordering.Domain._Shared.Attributes;

namespace AeroTech.Ordering.Domain._Shared.Enums
{
    [SpecRef("DD-ENUM-001")]
    public enum ItemType
    {
        [SpecRef("DD-ENUM-001")]
        Flight = 1,

        [SpecRef("DD-ENUM-001")]
        Seat = 2,

        [SpecRef("DD-ENUM-001")]
        Bag = 3,

        [SpecRef("DD-ENUM-001")]
        Meal = 4,

        [SpecRef("DD-ENUM-001")]
        Fee = 5,
    }
}
