using AeroTech.Ordering.Domain._Shared.Attributes;

namespace AeroTech.Ordering.Domain._Shared.Enums
{
    [SpecRef("DD-ENUM-018")]
    public enum CompensationPolicy
    {
        [SpecRef("DD-ENUM-018")]
        Compensate = 1,

        [SpecRef("DD-ENUM-018")]
        ForwardOnly = 2,
    }
}
