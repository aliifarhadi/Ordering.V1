using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Enums;
using AeroTech.Ordering.Domain._Shared.Identifiers;
using AeroTech.Ordering.Domain._Shared.Resources;
using AeroTech.Ordering.Domain._Shared.ValueObjects;
using AeroTech.Ordering.Domain.DeliveryRecordAggregate.ValueObjects;
using NodaTime;

namespace AeroTech.Ordering.Domain.DeliveryRecordAggregate.Entities
{
    [SpecRef("CDR-3.1.2")]
    public sealed class SegmentDelivery
    {
        private readonly List<UnitStatusChange> _statusHistory = [];

        private SegmentDelivery()
        {
        }

        [SpecRef("CDR-3.1.2")]
        internal SegmentDelivery(
            int unitNumber,
            OrderItemId orderItemRef,
            JourneyElementId journeyRef,
            FlightSegmentRef flight,
            Money unitValue)
        {
            UnitNumber = unitNumber;
            OrderItemRef = orderItemRef;
            JourneyRef = journeyRef;
            Flight = flight;
            UnitValue = unitValue;
            Status = SegmentDeliveryStatus.Open;
        }

        [SpecRef("CDR-3.1.2")]
        public int UnitNumber { get; private set; }

        [SpecRef("CDR-3.1.2")]
        public OrderItemId OrderItemRef { get; private set; }

        [SpecRef("CDR-3.1.2")]
        public JourneyElementId JourneyRef { get; private set; }

        [SpecRef("CDR-3.1.2")]
        public FlightSegmentRef Flight { get; private set; } = null!;

        [SpecRef("CDR-3.1.2")]
        public SegmentDeliveryStatus Status { get; private set; }

        [SpecRef("CDR-3.1.2")]
        public string? ControlHolder { get; private set; }

        [SpecRef("CDR-3.1.2")]
        public Instant? ControlAcquiredAtUtc { get; private set; }

        [SpecRef("CDR-3.1.2")]
        public Instant? ControlLeaseExpiresAtUtc { get; private set; }

        [SpecRef("CDR-3.1.2")]
        public Money UnitValue { get; private set; } = null!;

        [SpecRef("CDR-3.1.2")]
        public IReadOnlyList<UnitStatusChange> StatusHistory => _statusHistory.AsReadOnly();

        [SpecRef("I-26")]
        public bool IsUnderExternalControl => Status is
            SegmentDeliveryStatus.ControlTransferred or
            SegmentDeliveryStatus.CheckedIn or
            SegmentDeliveryStatus.Boarded;

        [SpecRef("CDR-3.1.7")]
        public bool IsTerminal => Status is
            SegmentDeliveryStatus.Exchanged or
            SegmentDeliveryStatus.Refunded or
            SegmentDeliveryStatus.Void;

        [SpecRef("CDR-3.1.7")]
        public bool IsConsumed => Status is
            SegmentDeliveryStatus.Flown or
            SegmentDeliveryStatus.NoShow;

        [SpecRef("CDR-3.1.4-TransferControl")]
        internal void TransferControl(ControlLease lease, Instant now, string source)
        {
            if (ControlHolder is not null)
                throw ExceptionFactory.ControlAlreadyHeld(UnitNumber, ControlHolder);

            TransitionTo(SegmentDeliveryStatus.ControlTransferred, now, source);

            ControlHolder = lease.Holder;
            ControlAcquiredAtUtc = lease.AcquiredAtUtc;
            ControlLeaseExpiresAtUtc = lease.ExpiresAtUtc;
        }

        [SpecRef("CDR-3.1.4-ReleaseControl")]
        internal void ReleaseControl(Instant now, string source)
        {
            if (ControlHolder is null)
                throw ExceptionFactory.ControlNotHeld(UnitNumber);

            TransitionTo(SegmentDeliveryStatus.Open, now, source);

            ControlHolder = null;
            ControlAcquiredAtUtc = null;
            ControlLeaseExpiresAtUtc = null;
        }

        [SpecRef("CDR-3.1.4-ApplyUnitStatusChange")]
        internal void ApplyStatusChange(SegmentDeliveryStatus target, Instant now, string source)
        {
            TransitionTo(target, now, source);

            if (target is SegmentDeliveryStatus.Flown or SegmentDeliveryStatus.NoShow)
            {
                ControlHolder = null;
                ControlAcquiredAtUtc = null;
                ControlLeaseExpiresAtUtc = null;
            }
        }

        [SpecRef("CDR-3.1.4-VoidRecord")]
        internal void Void(Instant now, string source) =>
            TransitionTo(SegmentDeliveryStatus.Void, now, source);

        [SpecRef("CDR-3.1.4-RefundUnits")]
        internal void Refund(Instant now, string source) =>
            TransitionTo(SegmentDeliveryStatus.Refunded, now, source);

        [SpecRef("CDR-3.1.4-CorrectUnitStatus")]
        internal void Correct(SegmentDeliveryStatus target, Instant now, string source)
        {
            RecordChange(Status, target, now, source);
            Status = target;
        }

        private void TransitionTo(SegmentDeliveryStatus target, Instant now, string source)
        {
            if (!IsTransitionPermitted(Status, target))
                throw ExceptionFactory.SegmentDeliveryCannotTransition(UnitNumber, Status, target);

            RecordChange(Status, target, now, source);
            Status = target;
        }

        private void RecordChange(
            SegmentDeliveryStatus from,
            SegmentDeliveryStatus to,
            Instant now,
            string source) =>
            _statusHistory.Add(new UnitStatusChange(_statusHistory.Count + 1, from, to, now, source));

        [SpecRef("CDR-3.1.5")]
        private static bool IsTransitionPermitted(SegmentDeliveryStatus from, SegmentDeliveryStatus to) =>
            (from, to) switch
            {
                (SegmentDeliveryStatus.Open, SegmentDeliveryStatus.ControlTransferred) => true,
                (SegmentDeliveryStatus.ControlTransferred, SegmentDeliveryStatus.Open) => true,
                (SegmentDeliveryStatus.ControlTransferred, SegmentDeliveryStatus.CheckedIn) => true,
                (SegmentDeliveryStatus.CheckedIn, SegmentDeliveryStatus.Boarded) => true,
                (SegmentDeliveryStatus.CheckedIn, SegmentDeliveryStatus.ControlTransferred) => true,
                (SegmentDeliveryStatus.Boarded, SegmentDeliveryStatus.Flown) => true,
                (SegmentDeliveryStatus.Boarded, SegmentDeliveryStatus.ControlTransferred) => true,
                (SegmentDeliveryStatus.ControlTransferred, SegmentDeliveryStatus.NoShow) => true,
                (SegmentDeliveryStatus.Open, SegmentDeliveryStatus.Exchanged) => true,
                (SegmentDeliveryStatus.Open, SegmentDeliveryStatus.Refunded) => true,
                (SegmentDeliveryStatus.Open, SegmentDeliveryStatus.Void) => true,
                (SegmentDeliveryStatus.Flown, SegmentDeliveryStatus.Refunded) => true,
                (SegmentDeliveryStatus.NoShow, SegmentDeliveryStatus.Refunded) => true,
                _ => false,
            };
    }
}
