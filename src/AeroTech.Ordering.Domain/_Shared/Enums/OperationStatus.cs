using AeroTech.Ordering.Domain._Shared.Attributes;

namespace AeroTech.Ordering.Domain._Shared.Enums
{
    [SpecRef("DD-ENUM-017")]
    public enum OperationStatus
    {
        [SpecRef("DD-ENUM-017")]
        Running = 1,

        [SpecRef("DD-ENUM-017")]
        AwaitingExternal = 2,

        [SpecRef("DD-ENUM-017")]
        Completed = 3,

        [SpecRef("DD-ENUM-017")]
        Failed = 4,

        [SpecRef("DD-ENUM-017")]
        AwaitingManualReview = 5,

        [SpecRef("DD-ENUM-017")]
        Compensated = 6,
    }
}
