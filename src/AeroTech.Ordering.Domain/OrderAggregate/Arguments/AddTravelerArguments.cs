using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Enums;
using AeroTech.Ordering.Domain._Shared.Identifiers;
using AeroTech.Ordering.Domain._Shared.ValueObjects;
using NodaTime;

namespace AeroTech.Ordering.Domain.OrderAggregate.Arguments
{
    [SpecRef("DD-CMD-002")]
    public sealed record AddTravelerArguments
    {
        [SpecRef("DD-CMD-002")]
        public required TravelerId TravelerId { get; init; }

        [SpecRef("DD-CMD-002")]
        public required TravelerName Name { get; init; }

        [SpecRef("DD-CMD-002")]
        public required PassengerTypeCode Ptc { get; init; }

        [SpecRef("DD-CMD-002")]
        public LocalDate? DateOfBirth { get; init; }

        [SpecRef("DD-CMD-002")]
        public required IReadOnlyList<IdentityDocument> Documents { get; init; }

        [SpecRef("DD-CMD-002")]
        public TravelerId? AssociatedAdult { get; init; }

        [SpecRef("DD-CMD-002")]
        public CustomerId? CustomerRef { get; init; }
    }
}
