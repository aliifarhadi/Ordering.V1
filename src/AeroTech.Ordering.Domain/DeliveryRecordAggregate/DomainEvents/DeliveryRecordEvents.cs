using AeroTech.Framework.Core.Domain.Events;
using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Enums;
using AeroTech.Ordering.Domain._Shared.Identifiers;

namespace AeroTech.Ordering.Domain.DeliveryRecordAggregate.DomainEvents
{
    [SpecRef("CDR-3.1.4-Issue")]
    public sealed record DeliveryRecordIssued(
        string EventId,
        string AggregateId,
        OrderId OrderRef,
        TravelerId TravelerRef,
        long AggregateVersion,
        DateTimeOffset TimeOfOccurrence)
        : DomainEvent(EventId, AggregateId, TimeOfOccurrence);

    [SpecRef("CDR-3.1.4-ApplyUnitStatusChange")]
    public sealed record UnitStatusChanged(
        string EventId,
        string AggregateId,
        int UnitNumber,
        SegmentDeliveryStatus Status,
        long AggregateVersion,
        DateTimeOffset TimeOfOccurrence)
        : DomainEvent(EventId, AggregateId, TimeOfOccurrence);

    [SpecRef("CDR-3.1.6")]
    public sealed record ServiceDeliveryConsumed(
        string EventId,
        string AggregateId,
        int UnitNumber,
        OrderItemId OrderItemRef,
        long AggregateVersion,
        DateTimeOffset TimeOfOccurrence)
        : DomainEvent(EventId, AggregateId, TimeOfOccurrence);

    [SpecRef("CDR-3.1.4-VoidRecord")]
    public sealed record DeliveryRecordVoided(
        string EventId,
        string AggregateId,
        OrderId OrderRef,
        long AggregateVersion,
        DateTimeOffset TimeOfOccurrence)
        : DomainEvent(EventId, AggregateId, TimeOfOccurrence);

    [SpecRef("CDR-3.1.4-RefundUnits")]
    public sealed record DeliveryUnitsRefunded(
        string EventId,
        string AggregateId,
        OrderId OrderRef,
        long AggregateVersion,
        DateTimeOffset TimeOfOccurrence)
        : DomainEvent(EventId, AggregateId, TimeOfOccurrence);

    [SpecRef("CDR-3.1.4-TransferControl")]
    public sealed record ControlTransferred(
        string EventId,
        string AggregateId,
        int UnitNumber,
        string Holder,
        long AggregateVersion,
        DateTimeOffset TimeOfOccurrence)
        : DomainEvent(EventId, AggregateId, TimeOfOccurrence);

    [SpecRef("CDR-3.1.4-ReleaseControl")]
    public sealed record ControlReleased(
        string EventId,
        string AggregateId,
        int UnitNumber,
        long AggregateVersion,
        DateTimeOffset TimeOfOccurrence)
        : DomainEvent(EventId, AggregateId, TimeOfOccurrence);

    [SpecRef("CDR-3.1.4-CorrectUnitStatus")]
    public sealed record UnitStatusCorrected(
        string EventId,
        string AggregateId,
        int UnitNumber,
        SegmentDeliveryStatus Status,
        string Justification,
        long AggregateVersion,
        DateTimeOffset TimeOfOccurrence)
        : DomainEvent(EventId, AggregateId, TimeOfOccurrence);
}
