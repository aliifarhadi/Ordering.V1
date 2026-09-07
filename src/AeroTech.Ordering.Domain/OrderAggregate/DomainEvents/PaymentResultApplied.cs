using AeroTech.Framework.Core.Domain.Events;
using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Enums;
using AeroTech.Ordering.Domain._Shared.Identifiers;

namespace AeroTech.Ordering.Domain.OrderAggregate.DomainEvents
{
    [SpecRef("DD-CMD-008")]
    public sealed record PaymentResultApplied(
        string EventId,
        string AggregateId,
        PaymentRecordId PaymentRecordId,
        PaymentStatus Status,
        long AggregateVersion,
        DateTimeOffset TimeOfOccurrence)
        : DomainEvent(EventId, AggregateId, TimeOfOccurrence);
}
