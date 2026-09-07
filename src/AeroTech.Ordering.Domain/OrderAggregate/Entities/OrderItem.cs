using AeroTech.Framework.Core.Domain.Entities;
using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Enums;
using AeroTech.Ordering.Domain._Shared.Identifiers;
using AeroTech.Ordering.Domain._Shared.Resources;
using AeroTech.Ordering.Domain._Shared.ValueObjects;
using AeroTech.Ordering.Domain.OrderAggregate.ValueObjects;

namespace AeroTech.Ordering.Domain.OrderAggregate.Entities
{
    [SpecRef("DD-ENT-004")]
    public sealed class OrderItem : Entity<OrderItemId>
    {
        private readonly List<JourneyElementId> _journeyRefs = [];

        private OrderItem()
        {
        }

        [SpecRef("DD-ENT-004")]
        internal OrderItem(
            OrderItemId id,
            ItemType type,
            TravelerId travelerRef,
            IReadOnlyList<JourneyElementId> journeyRefs,
            ProductRef product,
            PriceBreakdown price,
            OfferRef sourceOffer,
            OrderItemId? replacesItem,
            PartnerDelivery? partner)
        {
            if (type == ItemType.Flight && journeyRefs.Count < 1)
                throw ExceptionFactory.FlightItemRequiresJourneyReference();

            if (type != ItemType.Flight && journeyRefs.Count != 1)
                throw ExceptionFactory.AncillaryRequiresSingleJourneyReference();

            price.EnsureAllocationCoversJourneyElements(journeyRefs);

            Id = id;
            Type = type;
            Status = OrderItemStatus.Offered;
            TravelerRef = travelerRef;
            Product = product;
            Price = price;
            SourceOffer = sourceOffer;
            ReplacesItem = replacesItem;
            Partner = partner;
            _journeyRefs = [.. journeyRefs];
        }

        [SpecRef("DD-ENT-004-02")]
        public ItemType Type { get; private set; }

        [SpecRef("DD-ENT-004-03")]
        public OrderItemStatus Status { get; private set; }

        [SpecRef("DD-ENT-004-04")]
        public TravelerId TravelerRef { get; private set; }

        [SpecRef("DD-ENT-004-05")]
        public IReadOnlyList<JourneyElementId> JourneyRefs => _journeyRefs.AsReadOnly();

        [SpecRef("DD-ENT-004-06")]
        public ProductRef Product { get; private set; } = null!;

        [SpecRef("DD-ENT-004-07")]
        public PriceBreakdown Price { get; private set; } = null!;

        [SpecRef("DD-ENT-004-08")]
        public OfferRef SourceOffer { get; private set; } = null!;

        [SpecRef("DD-ENT-004-09")]
        public OrderItemId? ReplacesItem { get; private set; }

        [SpecRef("DD-ENT-004-10")]
        public PartnerDelivery? Partner { get; private set; }

        [SpecRef("DD-ENT-004-11")]
        public CancellationReason? CancellationReason { get; private set; }

        [SpecRef("DD-ENT-004-12")]
        public CreditAuthorityRef? CreditAuthority { get; private set; }

        [SpecRef("DD-ENT-004-03")]
        public bool IsActive => Status is not (OrderItemStatus.Cancelled or OrderItemStatus.Refunded);

        [SpecRef("DD-ENT-004-03")]
        public bool IsConfirmedOrBeyond => Status is
            OrderItemStatus.Confirmed or
            OrderItemStatus.Delivered or
            OrderItemStatus.PartiallyUsed or
            OrderItemStatus.Used or
            OrderItemStatus.Exchanged;

        [SpecRef("DD-ENT-004-03")]
        public bool IsDeliveredOrBeyond => Status is
            OrderItemStatus.Delivered or
            OrderItemStatus.PartiallyUsed or
            OrderItemStatus.Used;

        [SpecRef("DD-CMD-006")]
        internal void MoveToPendingPayment() => TransitionTo(OrderItemStatus.PendingPayment);

        [SpecRef("DD-CMD-008")]
        internal void MarkConfirmed() => TransitionTo(OrderItemStatus.Confirmed);

        [SpecRef("DD-CMD-010")]
        internal void RecordCreditAuthority(CreditAuthorityRef creditAuthority)
        {
            CreditAuthority = creditAuthority;
            TransitionTo(OrderItemStatus.Confirmed);
        }

        [SpecRef("DD-CMD-015")]
        internal void MarkDelivered(bool allocatedBalanceSettled)
        {
            if (!allocatedBalanceSettled && CreditAuthority is null)
                throw ExceptionFactory.DeliveryRequiresSettledOrCredit(Id);

            TransitionTo(OrderItemStatus.Delivered);
        }

        [SpecRef("DD-CMD-011")]
        internal void MarkCancelled(CancellationReason reason)
        {
            CancellationReason = reason;
            TransitionTo(OrderItemStatus.Cancelled);
        }

        [SpecRef("DD-ENT-004-03")]
        internal void MarkPartiallyUsed() => TransitionTo(OrderItemStatus.PartiallyUsed);

        [SpecRef("DD-ENT-004-03")]
        internal void MarkUsed() => TransitionTo(OrderItemStatus.Used);

        [SpecRef("CDR-2.1.9-ApplyRefundResult")]
        internal void MarkRefunded() => TransitionTo(OrderItemStatus.Refunded);

        [SpecRef("CDR-2.1.10")]
        internal void MarkExchanged() => TransitionTo(OrderItemStatus.Exchanged);

        [SpecRef("DD-ENT-004-05")]
        internal void ReplaceJourneyRef(JourneyElementId from, JourneyElementId to)
        {
            var index = _journeyRefs.IndexOf(from);
            if (index < 0)
                throw ExceptionFactory.JourneyElementNotFound(from);

            _journeyRefs[index] = to;
        }

        private void TransitionTo(OrderItemStatus target)
        {
            if (!IsTransitionPermitted(Status, target))
                throw ExceptionFactory.ItemCannotTransition(Id, Status, target);

            Status = target;
        }

        [SpecRef("DD-ENT-004-03")]
        private static bool IsTransitionPermitted(OrderItemStatus from, OrderItemStatus to) =>
            (from, to) switch
            {
                (OrderItemStatus.Offered, OrderItemStatus.PendingPayment) => true,
                (OrderItemStatus.Offered, OrderItemStatus.Cancelled) => true,
                (OrderItemStatus.PendingPayment, OrderItemStatus.Confirmed) => true,
                (OrderItemStatus.PendingPayment, OrderItemStatus.Cancelled) => true,
                (OrderItemStatus.Confirmed, OrderItemStatus.Delivered) => true,
                (OrderItemStatus.Confirmed, OrderItemStatus.Cancelled) => true,
                (OrderItemStatus.Confirmed, OrderItemStatus.Exchanged) => true,
                (OrderItemStatus.Delivered, OrderItemStatus.PartiallyUsed) => true,
                (OrderItemStatus.Delivered, OrderItemStatus.Used) => true,
                (OrderItemStatus.Delivered, OrderItemStatus.Cancelled) => true,
                (OrderItemStatus.Delivered, OrderItemStatus.Refunded) => true,
                (OrderItemStatus.Delivered, OrderItemStatus.Exchanged) => true,
                (OrderItemStatus.PartiallyUsed, OrderItemStatus.Used) => true,
                (OrderItemStatus.PartiallyUsed, OrderItemStatus.Refunded) => true,
                (OrderItemStatus.Used, OrderItemStatus.Refunded) => true,
                _ => false,
            };
    }
}
