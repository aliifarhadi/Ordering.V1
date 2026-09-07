using AeroTech.Ordering.Domain._Shared.Attributes;

namespace AeroTech.Ordering.Domain._Shared.Enums
{
    [SpecRef("DD-ENUM-019")]
    public enum ActorType
    {
        [SpecRef("DD-ENUM-019")]
        Customer = 1,

        [SpecRef("DD-ENUM-019")]
        Agent = 2,

        [SpecRef("DD-ENUM-019")]
        System = 3,

        [SpecRef("DD-ENUM-019")]
        Partner = 4,
    }
}
