using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Enums;
using AeroTech.Ordering.Domain._Shared.Identifiers;
using AeroTech.Ordering.Domain._Shared.ValueObjects;

namespace AeroTech.Ordering.Domain.OrderAggregate.Arguments
{
    [SpecRef("DD-CMD-015")]
    public sealed record ApplyDeliveryResultArguments
    {
        [SpecRef("DD-CMD-015")]
        public required IReadOnlyList<DeliveredItem> DeliveredItems { get; init; }
    }

    [SpecRef("DD-CMD-015")]
    public sealed record DeliveredItem
    {
        [SpecRef("DD-CMD-015")]
        public required OrderItemId OrderItemId { get; init; }

        [SpecRef("DD-CMD-015")]
        public required TravelerId TravelerRef { get; init; }

        [SpecRef("DD-CMD-015")]
        public required JourneyElementId JourneyRef { get; init; }

        [SpecRef("DD-CMD-015")]
        public required DeliveryUnitRef DeliveryRef { get; init; }
    }

    [SpecRef("CDR-2.1.9-ApplyRefundResult")]
    public sealed record ApplyRefundResultArguments
    {
        [SpecRef("CDR-2.1.9-ApplyRefundResult")]
        public required OrderItemId OrderItemId { get; init; }

        [SpecRef("CDR-8.3")]
        public required bool DeliveryUnitsWithdrawn { get; init; }
    }

    [SpecRef("DD-CMD-016")]
    public sealed record ApplyDeliveryStatusChangeArguments
    {
        [SpecRef("DD-CMD-016")]
        public required TravelerId TravelerRef { get; init; }

        [SpecRef("DD-CMD-016")]
        public required JourneyElementId JourneyRef { get; init; }

        [SpecRef("DD-CMD-016")]
        public required DeliveryStatus DeliveryStatus { get; init; }
    }

    [SpecRef("DD-CMD-017")]
    public sealed record RebookJourneyElementArguments
    {
        [SpecRef("DD-CMD-017")]
        public required JourneyElementId JourneyElementId { get; init; }

        [SpecRef("DD-CMD-017")]
        public required FlightSegmentRef Flight { get; init; }

        [SpecRef("DD-CMD-017")]
        public required CabinCode Cabin { get; init; }

        [SpecRef("DD-CMD-017")]
        public required RbdCode Rbd { get; init; }

        [SpecRef("DD-CMD-017")]
        public required bool NewSegmentConfirmed { get; init; }
    }

    [SpecRef("DD-CMD-018")]
    public sealed record ApplyScheduleChangeArguments
    {
        [SpecRef("DD-CMD-018")]
        public required JourneyElementId JourneyElementId { get; init; }

        [SpecRef("DD-CMD-018")]
        public required FlightSegmentRef Flight { get; init; }
    }
}
