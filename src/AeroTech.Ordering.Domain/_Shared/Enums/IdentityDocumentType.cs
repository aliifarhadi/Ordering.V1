using AeroTech.Ordering.Domain._Shared.Attributes;

namespace AeroTech.Ordering.Domain._Shared.Enums
{
    [SpecRef("DD-ENUM-011")]
    public enum IdentityDocumentType
    {
        [SpecRef("DD-ENUM-011")]
        NationalId = 1,

        [SpecRef("DD-ENUM-011")]
        Passport = 2,

        [SpecRef("DD-ENUM-011")]
        BirthCertificate = 3,
    }
}
