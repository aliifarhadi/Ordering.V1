using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.ValueObjects;

namespace AeroTech.Ordering.Domain.OrderAggregate.ValueObjects
{
    [SpecRef("DD-VO-026")]
    public sealed record OrderTotals
    {
#pragma warning disable CS8618
        private OrderTotals()
        {
        }
#pragma warning restore CS8618

        [SpecRef("DD-VO-026")]
        public OrderTotals(Money totalPrice, Money totalSettled, Money balanceDue)
        {
            TotalPrice = totalPrice;
            TotalSettled = totalSettled;
            BalanceDue = balanceDue;
        }

        [SpecRef("DD-VO-026")]
        public Money TotalPrice { get; }

        [SpecRef("DD-VO-026")]
        public Money TotalSettled { get; }

        [SpecRef("DD-VO-026")]
        public Money BalanceDue { get; }
    }
}
