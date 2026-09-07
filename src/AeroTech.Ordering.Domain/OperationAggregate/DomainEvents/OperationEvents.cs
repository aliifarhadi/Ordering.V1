using AeroTech.Framework.Core.Domain.Events;
using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Enums;

namespace AeroTech.Ordering.Domain.OperationAggregate.DomainEvents
{
    [SpecRef("DD-AGG-002")]
    public sealed record OperationStarted(
        string EventId,
        string AggregateId,
        OperationType Type,
        DateTimeOffset TimeOfOccurrence)
        : DomainEvent(EventId, AggregateId, TimeOfOccurrence);

    [SpecRef("DD-AGG-002-05")]
    public sealed record OperationStepAdvanced(
        string EventId,
        string AggregateId,
        string CurrentStep,
        DateTimeOffset TimeOfOccurrence)
        : DomainEvent(EventId, AggregateId, TimeOfOccurrence);

    [SpecRef("DD-AGG-002-04")]
    public sealed record OperationStatusChanged(
        string EventId,
        string AggregateId,
        OperationStatus Status,
        DateTimeOffset TimeOfOccurrence)
        : DomainEvent(EventId, AggregateId, TimeOfOccurrence);
}
