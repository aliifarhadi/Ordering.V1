using AeroTech.Ordering.Domain._Shared.Attributes;

namespace AeroTech.Ordering.Domain._Shared.Enums
{
    [SpecRef("DD-ENUM-022")]
    public enum ResidualPolicy
    {
        [SpecRef("DD-ENUM-022")]
        ProportionalToBase = 1,

        [SpecRef("DD-ENUM-022")]
        SuppliedExplicitly = 2,
    }
}
