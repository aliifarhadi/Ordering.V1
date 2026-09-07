using AeroTech.Ordering.Domain._Shared.Attributes;

namespace AeroTech.Ordering.Domain._Shared.Enums
{
    [SpecRef("DD-ENUM-012")]
    public enum ContactType
    {
        [SpecRef("DD-ENUM-012")]
        Email = 1,

        [SpecRef("DD-ENUM-012")]
        Mobile = 2,

        [SpecRef("DD-ENUM-012")]
        Phone = 3,
    }
}
