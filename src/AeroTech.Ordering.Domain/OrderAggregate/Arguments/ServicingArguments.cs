using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Enums;
using AeroTech.Ordering.Domain._Shared.Identifiers;
using AeroTech.Ordering.Domain._Shared.ValueObjects;
using NodaTime;

namespace AeroTech.Ordering.Domain.OrderAggregate.Arguments
{
    [SpecRef("DD-CMD-004")]
    public sealed record ContactPointArguments
    {
        [SpecRef("DD-CMD-004")]
        public required ContactPoint Contact { get; init; }
    }

    [SpecRef("DD-CMD-006")]
    public sealed record ConfirmOrderArguments
    {
        [SpecRef("DD-CMD-006")]
        public required TimeLimitId TimeLimitId { get; init; }

        [SpecRef("DD-CMD-006")]
        public required Instant PaymentDueAtUtc { get; init; }
    }

    [SpecRef("DD-CMD-011")]
    public sealed record CancelOrderItemArguments
    {
        [SpecRef("DD-CMD-011")]
        public required OrderItemId OrderItemId { get; init; }

        [SpecRef("DD-CMD-011")]
        public required CancellationReason Reason { get; init; }

        [SpecRef("DD-CMD-011")]
        public required bool DeliveryUnitsWithdrawn { get; init; }

        [SpecRef("DD-CMD-011")]
        public required bool ValueReturnCompletedOrReserved { get; init; }
    }

    [SpecRef("DD-CMD-012")]
    public sealed record CancelOrderArguments
    {
        [SpecRef("DD-CMD-012")]
        public required CancellationReason Reason { get; init; }

        [SpecRef("DD-CMD-012")]
        public required bool DeliveryUnitsWithdrawn { get; init; }

        [SpecRef("DD-CMD-012")]
        public required bool ValueReturnCompletedOrReserved { get; init; }
    }

    [SpecRef("DD-CMD-013")]
    public sealed record ExtendTimeLimitArguments
    {
        [SpecRef("DD-CMD-013")]
        public required TimeLimitId TimeLimitId { get; init; }

        [SpecRef("DD-CMD-013")]
        public required Instant DueAtUtc { get; init; }
    }

    [SpecRef("DD-CMD-014")]
    public sealed record ExpireTimeLimitArguments
    {
        [SpecRef("DD-CMD-014")]
        public required TimeLimitId TimeLimitId { get; init; }

        [SpecRef("DD-CMD-014")]
        public required IReadOnlyList<OrderItemId> ItemsCoveredByOpenPaymentCompletion { get; init; }
    }

    [SpecRef("DD-CMD-019")]
    public sealed record SuspensionArguments
    {
        [SpecRef("DD-CMD-019")]
        public required string Reason { get; init; }
    }
}
