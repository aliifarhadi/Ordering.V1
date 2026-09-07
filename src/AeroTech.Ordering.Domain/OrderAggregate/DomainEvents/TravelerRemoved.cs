using AeroTech.Framework.Core.Domain.Events;
using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Identifiers;

namespace AeroTech.Ordering.Domain.OrderAggregate.DomainEvents
{
    [SpecRef("CDR-2.1.9-RemoveTraveler")]
    public sealed record TravelerRemoved(
        string EventId,
        string AggregateId,
        TravelerId TravelerId,
        long AggregateVersion,
        DateTimeOffset TimeOfOccurrence)
        : DomainEvent(EventId, AggregateId, TimeOfOccurrence);
}
