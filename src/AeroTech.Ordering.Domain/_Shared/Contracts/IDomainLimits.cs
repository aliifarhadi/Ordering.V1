using AeroTech.Ordering.Domain._Shared.Attributes;

namespace AeroTech.Ordering.Domain._Shared.Contracts
{
    [SpecRef("DD-LIMITS-001")]
    [Blocked("BL-005", "Every maximum, minimum, and ceiling", "ADR-10 / O18 / ADR-09")]
    public interface IDomainLimits
    {
        [SpecRef("DD-LIMITS-001")]
        int MaxTravelersPerOrder { get; }

        [SpecRef("DD-LIMITS-001")]
        int MaxJourneyElementsPerOrder { get; }

        [SpecRef("DD-LIMITS-001")]
        int MaxItemsPerOrder { get; }

        [SpecRef("DD-LIMITS-001")]
        int MaxServicesPerTravelerSegment { get; }

        [SpecRef("DD-LIMITS-001")]
        int MaxTimeLimitExtensions { get; }

        [SpecRef("DD-LIMITS-001")]
        int MaxNameLength { get; }

        [SpecRef("DD-LIMITS-001")]
        int MaxRemarkLength { get; }
    }
}
