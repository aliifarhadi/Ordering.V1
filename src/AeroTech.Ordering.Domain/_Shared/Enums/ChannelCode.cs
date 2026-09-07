using AeroTech.Ordering.Domain._Shared.Attributes;

namespace AeroTech.Ordering.Domain._Shared.Enums
{
    [SpecRef("DD-ENUM-008")]
    public enum ChannelCode
    {
        [SpecRef("DD-ENUM-008")]
        DirectIbe = 1,

        [SpecRef("DD-ENUM-008")]
        CallCentre = 2,

        [SpecRef("DD-ENUM-008")]
        BackOffice = 3,

        [SpecRef("DD-ENUM-008")]
        Airport = 4,

        [SpecRef("DD-ENUM-008")]
        OtaApi = 5,

        [SpecRef("DD-ENUM-008")]
        OtaPanel = 6,
    }
}
