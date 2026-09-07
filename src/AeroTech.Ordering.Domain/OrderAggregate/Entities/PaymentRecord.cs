using AeroTech.Framework.Core.Domain.Entities;
using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Enums;
using AeroTech.Ordering.Domain._Shared.Identifiers;
using AeroTech.Ordering.Domain._Shared.Resources;
using AeroTech.Ordering.Domain._Shared.ValueObjects;
using AeroTech.Ordering.Domain.OrderAggregate.ValueObjects;
using NodaTime;

namespace AeroTech.Ordering.Domain.OrderAggregate.Entities
{
    [SpecRef("DD-ENT-005")]
    public sealed class PaymentRecord : Entity<PaymentRecordId>
    {
        private readonly List<PaymentAllocation> _allocations = [];

        private PaymentRecord()
        {
        }

        [SpecRef("DD-ENT-005")]
        internal PaymentRecord(
            PaymentRecordId id,
            FormOfPayment fop,
            Money amount,
            PaymentRequestRef? requestRef,
            FxSnapshot? fx,
            string? payerRef,
            Instant recordedAtUtc)
        {
            Id = id;
            Fop = fop;
            Amount = amount;
            Status = PaymentStatus.Pending;
            RequestRef = requestRef;
            Fx = fx;
            PayerRef = payerRef;
            RecordedAtUtc = recordedAtUtc;
            IsUnderDispute = false;
        }

        [SpecRef("DD-ENT-005")]
        public FormOfPayment Fop { get; private set; }

        [SpecRef("DD-ENT-005")]
        public Money Amount { get; private set; } = null!;

        [SpecRef("DD-ENT-005")]
        public PaymentStatus Status { get; private set; }

        [SpecRef("DD-ENT-005")]
        public PaymentRequestRef? RequestRef { get; private set; }

        [SpecRef("DD-ENT-005")]
        public IReadOnlyList<PaymentAllocation> Allocations => _allocations.AsReadOnly();

        [SpecRef("DD-ENT-005")]
        public FxSnapshot? Fx { get; private set; }

        [SpecRef("DD-ENT-005")]
        public string? PayerRef { get; private set; }

        [SpecRef("DD-ENT-005")]
        public Instant RecordedAtUtc { get; private set; }

        [SpecRef("DD-ENT-005")]
        public bool IsUnderDispute { get; private set; }

        [SpecRef("DD-ENT-005")]
        public bool IsSettled => Status is PaymentStatus.Settled or PaymentStatus.PartiallyRefunded;

        [SpecRef("DD-CMD-008")]
        internal void ApplyResult(PaymentStatus status) => Status = status;

        [SpecRef("DD-CMD-009")]
        internal void Allocate(PaymentAllocation allocation)
        {
            var allocated = _allocations.Aggregate(
                Money.Zero(Amount.Currency),
                (running, existing) => running + existing.Amount);

            if (allocated + allocation.Amount > Amount)
                throw ExceptionFactory.AllocationsCannotExceedAmount(Id);

            _allocations.Add(allocation);
        }

        [SpecRef("DD-ENT-005")]
        internal void MarkUnderDispute(bool isUnderDispute) => IsUnderDispute = isUnderDispute;

        [SpecRef("DD-ENT-005")]
        public Money AllocatedTo(OrderItemId itemRef) =>
            _allocations
                .Where(allocation => allocation.ItemRef == itemRef)
                .Aggregate(Money.Zero(Amount.Currency), (running, allocation) => running + allocation.Amount);
    }
}
