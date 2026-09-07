using AeroTech.Framework.Core.Domain.Events;
using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Enums;
using AeroTech.Ordering.Domain._Shared.Identifiers;

namespace AeroTech.Ordering.Domain.OrderAggregate.DomainEvents
{
    [SpecRef("DD-CMD-018")]
    public sealed record ScheduleChangeApplied(
        string EventId,
        string AggregateId,
        JourneyElementId JourneyElementId,
        long AggregateVersion,
        DateTimeOffset TimeOfOccurrence)
        : DomainEvent(EventId, AggregateId, TimeOfOccurrence);
}
