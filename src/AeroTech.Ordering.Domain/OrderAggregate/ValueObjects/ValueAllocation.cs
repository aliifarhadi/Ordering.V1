using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Enums;
using AeroTech.Ordering.Domain._Shared.Identifiers;
using AeroTech.Ordering.Domain._Shared.Resources;
using AeroTech.Ordering.Domain._Shared.ValueObjects;

namespace AeroTech.Ordering.Domain.OrderAggregate.ValueObjects
{
    [SpecRef("DD-VO-004")]
    public sealed record ValueAllocation
    {
#pragma warning disable CS8618
        private ValueAllocation()
        {
        }
#pragma warning restore CS8618

        [SpecRef("DD-VO-004")]
        public ValueAllocation(
            TravelerId travelerRef,
            JourneyElementId journeyRef,
            Money amount,
            string allocationVersion,
            ResidualPolicy residualPolicy)
        {
            if (string.IsNullOrWhiteSpace(allocationVersion))
                throw ExceptionFactory.IdentifierIsRequired(nameof(allocationVersion));

            TravelerRef = travelerRef;
            JourneyRef = journeyRef;
            Amount = amount;
            AllocationVersion = allocationVersion;
            ResidualPolicy = residualPolicy;
        }

        [SpecRef("DD-VO-004-01")]
        public TravelerId TravelerRef { get; }

        [SpecRef("DD-VO-004-02")]
        public JourneyElementId JourneyRef { get; }

        [SpecRef("DD-VO-004-03")]
        public Money Amount { get; }

        [SpecRef("DD-VO-004-04")]
        public string AllocationVersion { get; }

        [SpecRef("DD-VO-004-05")]
        public ResidualPolicy ResidualPolicy { get; }
    }
}
