using AeroTech.Framework.Core.Domain.Events;
using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Enums;
using AeroTech.Ordering.Domain._Shared.Identifiers;

namespace AeroTech.Ordering.Domain.TaxDocumentAggregate.DomainEvents
{
    [SpecRef("CDR-3.2.4-Issue")]
    public sealed record TaxDocumentIssued(
        string EventId,
        string AggregateId,
        TaxDocumentKind Kind,
        OrderId OrderRef,
        long AggregateVersion,
        DateTimeOffset TimeOfOccurrence)
        : DomainEvent(EventId, AggregateId, TimeOfOccurrence);

    [SpecRef("CDR-3.2.4-MarkSubmitted")]
    public sealed record TaxDocumentSubmitted(
        string EventId,
        string AggregateId,
        long AggregateVersion,
        DateTimeOffset TimeOfOccurrence)
        : DomainEvent(EventId, AggregateId, TimeOfOccurrence);

    [SpecRef("CDR-3.2.4-MarkAccepted")]
    public sealed record TaxDocumentAccepted(
        string EventId,
        string AggregateId,
        string ResponseReference,
        long AggregateVersion,
        DateTimeOffset TimeOfOccurrence)
        : DomainEvent(EventId, AggregateId, TimeOfOccurrence);

    [SpecRef("CDR-3.2.4-MarkRejected")]
    public sealed record TaxDocumentRejected(
        string EventId,
        string AggregateId,
        string RejectionReason,
        long AggregateVersion,
        DateTimeOffset TimeOfOccurrence)
        : DomainEvent(EventId, AggregateId, TimeOfOccurrence);

    [SpecRef("CDR-3.2.4-Correct")]
    public sealed record TaxDocumentCorrected(
        string EventId,
        string AggregateId,
        long AggregateVersion,
        DateTimeOffset TimeOfOccurrence)
        : DomainEvent(EventId, AggregateId, TimeOfOccurrence);
}
