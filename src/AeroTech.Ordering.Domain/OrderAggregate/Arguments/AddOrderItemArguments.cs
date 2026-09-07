using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Enums;
using AeroTech.Ordering.Domain._Shared.Identifiers;
using AeroTech.Ordering.Domain.OrderAggregate.ValueObjects;

namespace AeroTech.Ordering.Domain.OrderAggregate.Arguments
{
    [SpecRef("DD-CMD-005")]
    public sealed record AddOrderItemArguments
    {
        [SpecRef("DD-CMD-005")]
        public required OrderItemId OrderItemId { get; init; }

        [SpecRef("DD-CMD-005")]
        public required ItemType Type { get; init; }

        [SpecRef("DD-CMD-005")]
        public required TravelerId TravelerRef { get; init; }

        [SpecRef("DD-CMD-005")]
        public required IReadOnlyList<JourneyElementId> JourneyRefs { get; init; }

        [SpecRef("DD-CMD-005")]
        public required ProductRef Product { get; init; }

        [SpecRef("DD-CMD-005")]
        public required PriceBreakdown Price { get; init; }

        [SpecRef("DD-CMD-005")]
        public required OfferRef SourceOffer { get; init; }

        [SpecRef("DD-CMD-005")]
        public OrderItemId? ReplacesItem { get; init; }

        [SpecRef("DD-CMD-005")]
        public PartnerDelivery? Partner { get; init; }
    }
}
