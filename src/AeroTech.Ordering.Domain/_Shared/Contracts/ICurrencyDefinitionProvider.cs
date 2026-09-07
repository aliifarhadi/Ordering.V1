using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.ValueObjects;

namespace AeroTech.Ordering.Domain._Shared.Contracts
{
    [SpecRef("DD-CURRENCY-001")]
    [Blocked("BL-004", "Currency scale and increment", "O2")]
    public interface ICurrencyDefinitionProvider
    {
        [SpecRef("DD-CURRENCY-001")]
        int GetScale(CurrencyCode currency);

        [SpecRef("DD-CURRENCY-001")]
        decimal GetIncrement(CurrencyCode currency);
    }
}
