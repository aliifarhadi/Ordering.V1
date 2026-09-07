using AeroTech.Framework.Core.Domain.Events;
using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Enums;
using AeroTech.Ordering.Domain._Shared.Identifiers;

namespace AeroTech.Ordering.Domain.OrderAggregate.DomainEvents
{
    [SpecRef("DD-CMD-019")]
    public sealed record SuspensionChanged(
        string EventId,
        string AggregateId,
        bool IsSuspended,
        long AggregateVersion,
        DateTimeOffset TimeOfOccurrence)
        : DomainEvent(EventId, AggregateId, TimeOfOccurrence);
}
