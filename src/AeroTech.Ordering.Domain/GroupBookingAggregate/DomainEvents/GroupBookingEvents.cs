using AeroTech.Framework.Core.Domain.Events;
using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Enums;
using AeroTech.Ordering.Domain._Shared.Identifiers;

namespace AeroTech.Ordering.Domain.GroupBookingAggregate.DomainEvents
{
    [SpecRef("CDR-2.2.4-Create")]
    public sealed record GroupBookingCreated(
        string EventId,
        string AggregateId,
        GroupReference Reference,
        long AggregateVersion,
        DateTimeOffset TimeOfOccurrence)
        : DomainEvent(EventId, AggregateId, TimeOfOccurrence);

    [SpecRef("CDR-2.2.4-ConfirmBlocks")]
    public sealed record GroupBlocksConfirmed(
        string EventId,
        string AggregateId,
        long AggregateVersion,
        DateTimeOffset TimeOfOccurrence)
        : DomainEvent(EventId, AggregateId, TimeOfOccurrence);

    [SpecRef("CDR-2.2.4-RecordDeposit")]
    public sealed record GroupDepositRecorded(
        string EventId,
        string AggregateId,
        PaymentRecordId PaymentRecordId,
        long AggregateVersion,
        DateTimeOffset TimeOfOccurrence)
        : DomainEvent(EventId, AggregateId, TimeOfOccurrence);

    [SpecRef("CDR-2.2.4-AllocateName")]
    public sealed record GroupNameAllocated(
        string EventId,
        string AggregateId,
        SlotId SlotId,
        OrderId SpawnedOrderRef,
        long AggregateVersion,
        DateTimeOffset TimeOfOccurrence)
        : DomainEvent(EventId, AggregateId, TimeOfOccurrence);

    [SpecRef("CDR-2.2.4-DeallocateName")]
    public sealed record GroupNameDeallocated(
        string EventId,
        string AggregateId,
        SlotId SlotId,
        long AggregateVersion,
        DateTimeOffset TimeOfOccurrence)
        : DomainEvent(EventId, AggregateId, TimeOfOccurrence);

    [SpecRef("CDR-2.2.4-ReleaseSeats")]
    public sealed record GroupSeatsReleased(
        string EventId,
        string AggregateId,
        BlockId BlockId,
        int Seats,
        long AggregateVersion,
        DateTimeOffset TimeOfOccurrence)
        : DomainEvent(EventId, AggregateId, TimeOfOccurrence);

    [SpecRef("CDR-2.2.4-ExtendNameDeadline")]
    public sealed record GroupTimeLimitExtended(
        string EventId,
        string AggregateId,
        TimeLimitId TimeLimitId,
        long AggregateVersion,
        DateTimeOffset TimeOfOccurrence)
        : DomainEvent(EventId, AggregateId, TimeOfOccurrence);

    [SpecRef("CDR-2.2.4-CancelGroup")]
    public sealed record GroupCancelled(
        string EventId,
        string AggregateId,
        CancellationReason Reason,
        long AggregateVersion,
        DateTimeOffset TimeOfOccurrence)
        : DomainEvent(EventId, AggregateId, TimeOfOccurrence);
}
