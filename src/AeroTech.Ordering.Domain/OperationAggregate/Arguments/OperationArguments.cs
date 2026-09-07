using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Enums;
using AeroTech.Ordering.Domain._Shared.Identifiers;
using NodaTime;

namespace AeroTech.Ordering.Domain.OperationAggregate.Arguments
{
    [SpecRef("DD-AGG-002")]
    public sealed record StartOperationArguments
    {
        [SpecRef("DD-AGG-002-02")]
        public required OperationType Type { get; init; }

        [SpecRef("DD-AGG-002-03")]
        public OrderId? OrderRef { get; init; }

        [SpecRef("CDR-4.1.1-GroupRef")]
        public GroupBookingId? GroupRef { get; init; }

        [SpecRef("DD-AGG-002-05")]
        public required string CurrentStep { get; init; }

        [SpecRef("DD-AGG-002-08")]
        public required string CorrelationId { get; init; }

        [SpecRef("DD-AGG-002-09")]
        public string? CausationId { get; init; }

        [SpecRef("DD-AGG-002-12")]
        public Instant? TimeoutAtUtc { get; init; }

        [SpecRef("DD-AGG-002-13")]
        public required CompensationPolicy CompensationPolicy { get; init; }

        [SpecRef("DD-AGG-002-16")]
        public required string IdempotencyKey { get; init; }

        [SpecRef("DD-AGG-002-17")]
        public required IReadOnlyList<OrderItemId> ScopedItems { get; init; }
    }

    [SpecRef("DD-AGG-002-05")]
    public sealed record AdvanceOperationArguments
    {
        [SpecRef("DD-AGG-002-05")]
        public required string NextStep { get; init; }

        [SpecRef("DD-VO-029")]
        public string? PreviousStepOutcome { get; init; }
    }

    [SpecRef("DD-AGG-002-07")]
    public sealed record AwaitExternalArguments
    {
        [SpecRef("DD-AGG-002-07")]
        public required string ExpectedExternalMessage { get; init; }

        [SpecRef("DD-AGG-002-11")]
        public Instant? NextRetryAtUtc { get; init; }
    }
}
