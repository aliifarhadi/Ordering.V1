using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Enums;
using AeroTech.Ordering.Domain._Shared.Identifiers;
using AeroTech.Ordering.Domain._Shared.ValueObjects;
using AeroTech.Ordering.Domain.OrderAggregate.ValueObjects;

namespace AeroTech.Ordering.Domain.OrderAggregate.Arguments
{
    [SpecRef("DD-CMD-007")]
    public sealed record RecordPaymentRequestArguments
    {
        [SpecRef("DD-CMD-007")]
        public required PaymentRecordId PaymentRecordId { get; init; }

        [SpecRef("DD-CMD-007")]
        public required FormOfPayment Fop { get; init; }

        [SpecRef("DD-CMD-007")]
        public required Money Amount { get; init; }

        [SpecRef("DD-CMD-007")]
        public PaymentRequestRef? RequestRef { get; init; }

        [SpecRef("DD-CMD-007")]
        public FxSnapshot? Fx { get; init; }

        [SpecRef("DD-CMD-007")]
        public string? PayerRef { get; init; }
    }

    [SpecRef("DD-CMD-008")]
    public sealed record ApplyPaymentResultArguments
    {
        [SpecRef("DD-CMD-008")]
        public required PaymentRecordId PaymentRecordId { get; init; }

        [SpecRef("DD-CMD-008")]
        public required PaymentStatus Status { get; init; }
    }

    [SpecRef("DD-CMD-009")]
    public sealed record AllocatePaymentArguments
    {
        [SpecRef("DD-CMD-009")]
        public required PaymentRecordId PaymentRecordId { get; init; }

        [SpecRef("DD-CMD-009")]
        public required IReadOnlyList<PaymentAllocation> Allocations { get; init; }
    }

    [SpecRef("DD-CMD-010")]
    public sealed record RecordCreditAuthorityArguments
    {
        [SpecRef("DD-CMD-010")]
        public required IReadOnlyList<OrderItemId> ItemRefs { get; init; }

        [SpecRef("DD-CMD-010")]
        public required CreditAuthorityRef CreditAuthority { get; init; }
    }
}
