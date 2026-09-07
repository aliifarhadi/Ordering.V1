using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Identifiers;
using AeroTech.Ordering.Domain._Shared.ValueObjects;

namespace AeroTech.Ordering.Domain.OrderAggregate.Arguments
{
    [SpecRef("DD-ENT-002")]
    public sealed record AddJourneyElementArguments
    {
        [SpecRef("DD-ENT-002")]
        public required JourneyElementId JourneyElementId { get; init; }

        [SpecRef("DD-ENT-002")]
        public required FlightSegmentRef Flight { get; init; }

        [SpecRef("DD-ENT-002")]
        public required CabinCode Cabin { get; init; }

        [SpecRef("DD-ENT-002")]
        public required RbdCode Rbd { get; init; }

        [SpecRef("DD-ENT-002")]
        public MarriedGroupId? MarriedGroup { get; init; }

        [SpecRef("DD-ENT-002")]
        public required int SequenceInJourney { get; init; }
    }
}
