using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Identifiers;
using AeroTech.Ordering.Domain._Shared.ValueObjects;

namespace AeroTech.Ordering.Domain.OrderAggregate.ValueObjects
{
    [SpecRef("DD-VO-027")]
    public sealed record PaymentAllocation
    {
#pragma warning disable CS8618
        private PaymentAllocation()
        {
        }
#pragma warning restore CS8618

        [SpecRef("DD-VO-027")]
        public PaymentAllocation(OrderItemId itemRef, Money amount)
        {
            ItemRef = itemRef;
            Amount = amount;
        }

        [SpecRef("DD-VO-027")]
        public OrderItemId ItemRef { get; }

        [SpecRef("DD-VO-027")]
        public Money Amount { get; }
    }
}
