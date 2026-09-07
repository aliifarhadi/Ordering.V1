using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Enums;
using AeroTech.Ordering.Domain._Shared.ValueObjects;
using AeroTech.Ordering.Domain.OrderAggregate.Entities;
using NodaTime;

namespace AeroTech.Ordering.Domain.OrderAggregate.Specifications
{
    [SpecRef("DD-ENUM-003")]
    public static class OrderStatusSpecification
    {
        [SpecRef("DD-AGG-001-05")]
        public static OrderStatus Derive(
            IReadOnlyList<OrderItem> items,
            Instant? closedAtUtc,
            Money balanceDue)
        {
            if (closedAtUtc is not null)
                return OrderStatus.Closed;

            if (items.Count > 0 && items.All(item =>
                    item.Status is OrderItemStatus.Cancelled or OrderItemStatus.Refunded))
                return OrderStatus.Cancelled;

            var flightItems = items.Where(item => item.Type == ItemType.Flight).ToList();

            if (flightItems.Count > 0 && flightItems.All(item => item.Status == OrderItemStatus.Used))
                return OrderStatus.Flown;

            if (flightItems.Any(item => item.Status is OrderItemStatus.Used or OrderItemStatus.PartiallyUsed) &&
                flightItems.Any(item => item.Status is not (OrderItemStatus.Used or OrderItemStatus.PartiallyUsed)))
                return OrderStatus.PartiallyFlown;

            var activeItems = items.Where(item => item.IsActive).ToList();

            if (activeItems.Count > 0 && activeItems.All(item => item.IsDeliveredOrBeyond))
                return OrderStatus.Delivered;

            if (flightItems.Any(item => item.Status == OrderItemStatus.Confirmed) &&
                balanceDue.Amount <= decimal.Zero)
                return OrderStatus.Confirmed;

            if (items.Any(item => item.Status == OrderItemStatus.PendingPayment))
                return OrderStatus.Pending;

            return OrderStatus.Draft;
        }
    }
}
