using AeroTech.Framework.Core.Domain.Aggregates;
using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Enums;
using AeroTech.Ordering.Domain._Shared.Identifiers;
using AeroTech.Ordering.Domain._Shared.Resources;
using AeroTech.Ordering.Domain._Shared.ValueObjects;
using AeroTech.Ordering.Domain.DeliveryRecordAggregate.Arguments;
using AeroTech.Ordering.Domain.DeliveryRecordAggregate.DomainEvents;
using AeroTech.Ordering.Domain.DeliveryRecordAggregate.Entities;
using AeroTech.Ordering.Domain.DeliveryRecordAggregate.Specifications;
using AeroTech.Ordering.Domain.DeliveryRecordAggregate.ValueObjects;
using NodaTime;
using IClock = AeroTech.Framework.Core.ServiceContracts.IClock;
using IIdGenerator = AeroTech.Framework.Core.ServiceContracts.IIdGenerator;

namespace AeroTech.Ordering.Domain.DeliveryRecordAggregate
{
    [SpecRef("CDR-3.1.1")]
    public sealed class DeliveryRecord : AggregateRoot<DeliveryRecordNumber>
    {
        private readonly List<SegmentDelivery> _segmentUnits = [];
        private readonly List<ServiceDelivery> _serviceUnits = [];

        private DeliveryRecord()
        {
        }

        private DeliveryRecord(
            DeliveryRecordNumber number,
            IssueDeliveryRecordArguments arguments,
            Instant issuedAtUtc)
        {
            Id = number;
            OrderRef = arguments.OrderRef;
            TravelerRef = arguments.TravelerRef;
            TravelerSnapshot = arguments.TravelerSnapshot;
            BranchRef = arguments.BranchRef;
            IssuedAtUtc = issuedAtUtc;
            VoidWindowHours = arguments.VoidWindowHours;
            AggregateVersion = 0;
        }

        [SpecRef("CDR-3.1.1")]
        public DeliveryRecordNumber Number => Id;

        [SpecRef("CDR-3.1.1")]
        public OrderId OrderRef { get; private set; }

        [SpecRef("CDR-3.1.1")]
        public TravelerId TravelerRef { get; private set; }

        [SpecRef("CDR-3.1.1")]
        public TravelerSnapshot TravelerSnapshot { get; private set; } = null!;

        [SpecRef("CDR-3.1.1")]
        public BranchId BranchRef { get; private set; }

        [SpecRef("CDR-3.1.1")]
        public Instant IssuedAtUtc { get; private set; }

        [SpecRef("I-33")]
        public int VoidWindowHours { get; private set; }

        [SpecRef("CDR-3.1.1")]
        public long AggregateVersion { get; private set; }

        [SpecRef("CDR-3.1.7")]
        public DeliveryRecordStatus Status =>
            DeliveryRecordStatusSpecification.Derive(SegmentUnits, ServiceUnits);

        [SpecRef("CDR-3.1.1")]
        public IReadOnlyList<SegmentDelivery> SegmentUnits => _segmentUnits.AsReadOnly();

        [SpecRef("CDR-3.1.1")]
        public IReadOnlyList<ServiceDelivery> ServiceUnits => _serviceUnits.AsReadOnly();

        [SpecRef("CDR-3.1.4-Issue")]
        public static DeliveryRecord Issue(
            DeliveryRecordNumber number,
            IssueDeliveryRecordArguments arguments,
            IIdGenerator idGenerator,
            IClock clock)
        {
            if (arguments.SegmentUnits.Count == 0 && arguments.ServiceUnits.Count == 0)
                throw ExceptionFactory.DeliveryRecordRequiresAtLeastOneUnit();

            var now = Instant.FromDateTimeOffset(clock.GetDateTime());
            var record = new DeliveryRecord(number, arguments, now);

            var unitNumber = 1;

            foreach (var unit in arguments.SegmentUnits)
            {
                record._segmentUnits.Add(new SegmentDelivery(
                    unitNumber: unitNumber++,
                    orderItemRef: unit.OrderItemRef,
                    journeyRef: unit.JourneyRef,
                    flight: unit.Flight,
                    unitValue: unit.UnitValue));
            }

            foreach (var unit in arguments.ServiceUnits)
            {
                record._serviceUnits.Add(new ServiceDelivery(
                    unitNumber: unitNumber++,
                    orderItemRef: unit.OrderItemRef,
                    serviceCode: unit.ServiceCode,
                    description: unit.Description,
                    unitValue: unit.UnitValue,
                    associatedSegmentUnit: unit.AssociatedSegmentUnit));
            }

            record.AggregateVersion++;

            record.Causes(new DeliveryRecordIssued(
                EventId: idGenerator.NewId().ToString(),
                AggregateId: record.Id.ToString(),
                OrderRef: record.OrderRef,
                TravelerRef: record.TravelerRef,
                AggregateVersion: record.AggregateVersion,
                TimeOfOccurrence: clock.GetDateTime()));

            return record;
        }

        [SpecRef("I-34")]
        public void EnsureUnitValuesMatchAllocation(Money expectedTotal)
        {
            if (_segmentUnits.Count == 0)
                return;

            var sum = _segmentUnits.Aggregate(
                Money.Zero(expectedTotal.Currency),
                (running, unit) => running + unit.UnitValue);

            if (sum != expectedTotal)
                throw ExceptionFactory.UnitValuesMustMatchAllocation();
        }

        [SpecRef("CDR-3.1.4-TransferControl")]
        public void TransferControl(
            TransferControlArguments arguments,
            IIdGenerator idGenerator,
            IClock clock)
        {
            var now = Instant.FromDateTimeOffset(clock.GetDateTime());
            var unit = RequireSegmentUnit(arguments.UnitNumber);

            unit.TransferControl(arguments.Lease, now, arguments.Lease.Holder);
            AggregateVersion++;

            Causes(new ControlTransferred(
                EventId: idGenerator.NewId().ToString(),
                AggregateId: Id.ToString(),
                UnitNumber: unit.UnitNumber,
                Holder: arguments.Lease.Holder,
                AggregateVersion: AggregateVersion,
                TimeOfOccurrence: clock.GetDateTime()));
        }

        [SpecRef("CDR-3.1.4-ReleaseControl")]
        public void ReleaseControl(
            ReleaseControlArguments arguments,
            IIdGenerator idGenerator,
            IClock clock)
        {
            var now = Instant.FromDateTimeOffset(clock.GetDateTime());
            var unit = RequireSegmentUnit(arguments.UnitNumber);
            var holder = unit.ControlHolder ?? throw ExceptionFactory.ControlNotHeld(unit.UnitNumber);

            unit.ReleaseControl(now, holder);
            AggregateVersion++;

            Causes(new ControlReleased(
                EventId: idGenerator.NewId().ToString(),
                AggregateId: Id.ToString(),
                UnitNumber: unit.UnitNumber,
                AggregateVersion: AggregateVersion,
                TimeOfOccurrence: clock.GetDateTime()));
        }

        [SpecRef("CDR-3.1.4-ApplyUnitStatusChange")]
        public void ApplyUnitStatusChange(
            ApplyUnitStatusChangeArguments arguments,
            IIdGenerator idGenerator,
            IClock clock)
        {
            var now = Instant.FromDateTimeOffset(clock.GetDateTime());
            var unit = RequireSegmentUnit(arguments.UnitNumber);

            unit.ApplyStatusChange(arguments.Status, now, arguments.Source);
            AggregateVersion++;

            Causes(new UnitStatusChanged(
                EventId: idGenerator.NewId().ToString(),
                AggregateId: Id.ToString(),
                UnitNumber: unit.UnitNumber,
                Status: unit.Status,
                AggregateVersion: AggregateVersion,
                TimeOfOccurrence: clock.GetDateTime()));
        }

        [SpecRef("CDR-3.1.6")]
        public void ConsumeServiceUnit(
            ConsumeServiceUnitArguments arguments,
            IIdGenerator idGenerator,
            IClock clock)
        {
            var unit = RequireServiceUnit(arguments.UnitNumber);

            unit.Consume();
            AggregateVersion++;

            Causes(new ServiceDeliveryConsumed(
                EventId: idGenerator.NewId().ToString(),
                AggregateId: Id.ToString(),
                UnitNumber: unit.UnitNumber,
                OrderItemRef: unit.OrderItemRef,
                AggregateVersion: AggregateVersion,
                TimeOfOccurrence: clock.GetDateTime()));
        }

        [SpecRef("CDR-3.1.4-VoidRecord")]
        public void VoidRecord(
            VoidRecordArguments arguments,
            IIdGenerator idGenerator,
            IClock clock)
        {
            var now = Instant.FromDateTimeOffset(clock.GetDateTime());

            EnsureWithinVoidWindow(now);

            if (_segmentUnits.Any(unit => unit.Status != SegmentDeliveryStatus.Open) ||
                _serviceUnits.Any(unit => !unit.IsOpen))
                throw ExceptionFactory.AllUnitsMustBeOpenToVoid(Id);

            foreach (var unit in _segmentUnits)
                unit.Void(now, arguments.Source);

            foreach (var unit in _serviceUnits)
                unit.Void();

            AggregateVersion++;

            Causes(new DeliveryRecordVoided(
                EventId: idGenerator.NewId().ToString(),
                AggregateId: Id.ToString(),
                OrderRef: OrderRef,
                AggregateVersion: AggregateVersion,
                TimeOfOccurrence: clock.GetDateTime()));
        }

        [SpecRef("CDR-3.1.4-RefundUnits")]
        public void RefundUnits(
            RefundUnitsArguments arguments,
            IIdGenerator idGenerator,
            IClock clock)
        {
            var now = Instant.FromDateTimeOffset(clock.GetDateTime());

            foreach (var unitNumber in arguments.SegmentUnitNumbers)
            {
                var unit = RequireSegmentUnit(unitNumber);
                EnsureNotUnderExternalControl(unit);
                unit.Refund(now, arguments.Source);
            }

            foreach (var unitNumber in arguments.ServiceUnitNumbers)
                RequireServiceUnit(unitNumber).Refund();

            AggregateVersion++;

            Causes(new DeliveryUnitsRefunded(
                EventId: idGenerator.NewId().ToString(),
                AggregateId: Id.ToString(),
                OrderRef: OrderRef,
                AggregateVersion: AggregateVersion,
                TimeOfOccurrence: clock.GetDateTime()));
        }

        [SpecRef("CDR-3.1.4-CorrectUnitStatus")]
        public void CorrectUnitStatus(
            CorrectUnitStatusArguments arguments,
            IIdGenerator idGenerator,
            IClock clock)
        {
            if (string.IsNullOrWhiteSpace(arguments.Justification))
                throw ExceptionFactory.IdentifierIsRequired(nameof(arguments.Justification));

            var now = Instant.FromDateTimeOffset(clock.GetDateTime());
            var unit = RequireSegmentUnit(arguments.UnitNumber);

            unit.Correct(arguments.Status, now, arguments.Justification);
            AggregateVersion++;

            Causes(new UnitStatusCorrected(
                EventId: idGenerator.NewId().ToString(),
                AggregateId: Id.ToString(),
                UnitNumber: unit.UnitNumber,
                Status: unit.Status,
                Justification: arguments.Justification,
                AggregateVersion: AggregateVersion,
                TimeOfOccurrence: clock.GetDateTime()));
        }

        [SpecRef("I-33")]
        private void EnsureWithinVoidWindow(Instant now)
        {
            if (now > IssuedAtUtc.Plus(Duration.FromHours(VoidWindowHours)))
                throw ExceptionFactory.VoidWindowExpired(Id);
        }

        [SpecRef("I-26")]
        private static void EnsureNotUnderExternalControl(SegmentDelivery unit)
        {
            if (unit.IsUnderExternalControl)
                throw ExceptionFactory.DeliveryUnitUnderExternalControl(
                    unit.UnitNumber,
                    unit.ControlHolder ?? unit.Status.ToString());
        }

        private SegmentDelivery RequireSegmentUnit(int unitNumber) =>
            _segmentUnits.FirstOrDefault(unit => unit.UnitNumber == unitNumber)
            ?? throw ExceptionFactory.SegmentUnitNotFound(unitNumber);

        private ServiceDelivery RequireServiceUnit(int unitNumber) =>
            _serviceUnits.FirstOrDefault(unit => unit.UnitNumber == unitNumber)
            ?? throw ExceptionFactory.ServiceUnitNotFound(unitNumber);
    }
}
