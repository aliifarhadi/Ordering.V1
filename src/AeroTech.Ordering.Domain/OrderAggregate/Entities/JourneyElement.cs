using AeroTech.Framework.Core.Domain.Entities;
using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Identifiers;
using AeroTech.Ordering.Domain._Shared.ValueObjects;

namespace AeroTech.Ordering.Domain.OrderAggregate.Entities
{
    [SpecRef("DD-ENT-002")]
    public sealed class JourneyElement : Entity<JourneyElementId>
    {
        private JourneyElement()
        {
        }

        [SpecRef("DD-ENT-002")]
        internal JourneyElement(
            JourneyElementId id,
            FlightSegmentRef flight,
            CabinCode cabin,
            RbdCode rbd,
            MarriedGroupId? marriedGroup,
            int sequenceInJourney)
        {
            Id = id;
            Flight = flight;
            Cabin = cabin;
            Rbd = rbd;
            MarriedGroup = marriedGroup;
            SequenceInJourney = sequenceInJourney;
        }

        [SpecRef("DD-ENT-002")]
        public FlightSegmentRef Flight { get; private set; } = null!;

        [SpecRef("DD-ENT-002")]
        public CabinCode Cabin { get; private set; }

        [SpecRef("DD-ENT-002")]
        public RbdCode Rbd { get; private set; }

        [SpecRef("DD-ENT-002")]
        public MarriedGroupId? MarriedGroup { get; private set; }

        [SpecRef("DD-ENT-002")]
        public int SequenceInJourney { get; private set; }

        [SpecRef("DD-CMD-018")]
        internal void ApplyScheduleChange(FlightSegmentRef flight)
        {
            Flight = flight;
        }

        [SpecRef("DD-CMD-017")]
        internal void Rebook(FlightSegmentRef flight, CabinCode cabin, RbdCode rbd)
        {
            Flight = flight;
            Cabin = cabin;
            Rbd = rbd;
        }
    }
}
