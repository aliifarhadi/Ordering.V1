using AeroTech.Ordering.Domain._Shared.Attributes;

namespace AeroTech.Ordering.Domain._Shared.Enums
{
    [SpecRef("CDR-ENUM-RangeStatus")]
    public enum RangeStatus
    {
        [SpecRef("CDR-ENUM-RangeStatus")]
        Active = 1,

        [SpecRef("CDR-ENUM-RangeStatus")]
        Exhausted = 2,

        [SpecRef("CDR-ENUM-RangeStatus")]
        Closed = 3,
    }
}
