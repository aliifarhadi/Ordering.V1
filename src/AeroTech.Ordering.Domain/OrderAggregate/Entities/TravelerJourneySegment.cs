using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Enums;
using AeroTech.Ordering.Domain._Shared.Identifiers;
using AeroTech.Ordering.Domain._Shared.Resources;
using AeroTech.Ordering.Domain._Shared.ValueObjects;

namespace AeroTech.Ordering.Domain.OrderAggregate.Entities
{
    [SpecRef("DD-ENT-003")]
    public sealed class TravelerJourneySegment
    {
        private TravelerJourneySegment()
        {
        }

        [SpecRef("DD-ENT-003")]
        internal TravelerJourneySegment(TravelerId travelerRef, JourneyElementId journeyRef)
        {
            TravelerRef = travelerRef;
            JourneyRef = journeyRef;
            SegmentStatus = SegmentStatus.Requested;
            DeliveryStatus = DeliveryStatus.NotStarted;
        }

        [SpecRef("DD-ENT-003-01")]
        public TravelerId TravelerRef { get; private set; }

        [SpecRef("DD-ENT-003-02")]
        public JourneyElementId JourneyRef { get; private set; }

        [SpecRef("DD-ENT-003-03")]
        public SegmentStatus SegmentStatus { get; private set; }

        [SpecRef("DD-ENT-003-04")]
        public DeliveryStatus DeliveryStatus { get; private set; }

        [SpecRef("DD-ENT-003-05")]
        public InventoryHoldRef? Hold { get; private set; }

        [SpecRef("DD-ENT-003-06")]
        public SeatAssignment? Seat { get; private set; }

        [SpecRef("DD-ENT-003-07")]
        public DeliveryUnitRef? DeliveryRef { get; private set; }

        [SpecRef("DD-ENT-003-03")]
        internal void TransitionSegmentStatus(SegmentStatus target)
        {
            if (!IsSegmentTransitionPermitted(SegmentStatus, target))
                throw ExceptionFactory.SegmentCannotTransition(SegmentStatus, target);

            SegmentStatus = target;
        }

        [SpecRef("DD-ENT-003-05")]
        internal void AssignHold(InventoryHoldRef hold) => Hold = hold;

        [SpecRef("DD-ENT-003-05")]
        internal void ReleaseHold() => Hold = null;

        [SpecRef("DD-ENT-003-06")]
        internal void AssignSeat(SeatAssignment seat) => Seat = seat;

        [SpecRef("DD-ENT-003-04")]
        internal void MirrorDeliveryStatus(DeliveryStatus target)
        {
            if (!IsDeliveryTransitionPermitted(DeliveryStatus, target))
                throw ExceptionFactory.DeliveryStatusCannotTransition(DeliveryStatus, target);

            DeliveryStatus = target;
        }

        [SpecRef("DD-ENT-003-07")]
        internal void MirrorDeliveryRef(DeliveryUnitRef deliveryRef) => DeliveryRef = deliveryRef;

        [SpecRef("DD-ENT-003-04")]
        public bool IsUnderExternalControl =>
            DeliveryStatus is DeliveryStatus.CheckedIn or DeliveryStatus.Boarded;

        [SpecRef("DD-ENT-003-04")]
        public bool IsConsumed =>
            DeliveryStatus is DeliveryStatus.Flown or DeliveryStatus.NoShow;

        private static bool IsSegmentTransitionPermitted(SegmentStatus from, SegmentStatus to) =>
            (from, to) switch
            {
                (SegmentStatus.Requested, SegmentStatus.Confirmed) => true,
                (SegmentStatus.Requested, SegmentStatus.Cancelled) => true,
                (SegmentStatus.Confirmed, SegmentStatus.Rebooked) => true,
                (SegmentStatus.Confirmed, SegmentStatus.Cancelled) => true,
                (SegmentStatus.Rebooked, SegmentStatus.Cancelled) => true,
                _ => false,
            };

        private static bool IsDeliveryTransitionPermitted(DeliveryStatus from, DeliveryStatus to) =>
            (from, to) switch
            {
                (DeliveryStatus.NotStarted, DeliveryStatus.CheckedIn) => true,
                (DeliveryStatus.NotStarted, DeliveryStatus.NoShow) => true,
                (DeliveryStatus.CheckedIn, DeliveryStatus.Boarded) => true,
                (DeliveryStatus.CheckedIn, DeliveryStatus.NotStarted) => true,
                (DeliveryStatus.Boarded, DeliveryStatus.Flown) => true,
                (DeliveryStatus.Boarded, DeliveryStatus.NotStarted) => true,
                _ => false,
            };
    }
}
