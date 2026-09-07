using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Identifiers;

namespace AeroTech.Ordering.Domain._Shared.Contracts
{
    [SpecRef("DD-REFGEN-001")]
    [Blocked("BL-003", "OrderReference format and alphabet", "O22")]
    public interface IOrderReferenceGenerator
    {
        [SpecRef("DD-REFGEN-001")]
        OrderReference Next();
    }
}
