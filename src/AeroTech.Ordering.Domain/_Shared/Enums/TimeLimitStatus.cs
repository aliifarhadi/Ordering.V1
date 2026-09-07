using AeroTech.Ordering.Domain._Shared.Attributes;

namespace AeroTech.Ordering.Domain._Shared.Enums
{
    [SpecRef("DD-ENUM-014")]
    public enum TimeLimitStatus
    {
        [SpecRef("DD-ENUM-014")]
        Active = 1,

        [SpecRef("DD-ENUM-014")]
        Met = 2,

        [SpecRef("DD-ENUM-014")]
        Expired = 3,

        [SpecRef("DD-ENUM-014")]
        Cancelled = 4,
    }
}
