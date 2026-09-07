using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.ValueObjects;

namespace AeroTech.Ordering.Domain.OrderAggregate.Arguments
{
    [SpecRef("DD-CMD-001")]
    public sealed record CreateOrderArguments
    {
        [SpecRef("DD-CMD-001")]
        public required CurrencyCode SaleCurrency { get; init; }
    }
}
