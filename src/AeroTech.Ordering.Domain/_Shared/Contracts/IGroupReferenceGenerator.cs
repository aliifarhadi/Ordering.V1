using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Identifiers;

namespace AeroTech.Ordering.Domain._Shared.Contracts
{
    [SpecRef("CDR-1.1-GroupReference")]
    [Blocked("BL-003", "GroupReference format and alphabet", "O22")]
    public interface IGroupReferenceGenerator
    {
        [SpecRef("CDR-1.1-GroupReference")]
        GroupReference Next();
    }
}
