using AeroTech.Ordering.Domain._Shared.Attributes;

namespace AeroTech.Ordering.Domain._Shared.Enums
{
    [SpecRef("DD-ENUM-021")]
    public enum ControlTransferState
    {
        [SpecRef("DD-ENUM-021")]
        Held = 1,

        [SpecRef("DD-ENUM-021")]
        TransferredOut = 2,

        [SpecRef("DD-ENUM-021")]
        TransferredIn = 3,
    }
}
