using AeroTech.Framework.Core.Domain.Repository;
using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Enums;
using AeroTech.Ordering.Domain._Shared.Identifiers;

namespace AeroTech.Ordering.Domain.DocumentNumberRangeAggregate.Contracts
{
    [SpecRef("CDR-3.3.1")]
    public interface IDocumentNumberRangeRepository : IRepository<DocumentNumberRange, RangeId>
    {
        [SpecRef("I-30")]
        Task<DocumentNumberRange?> GetActiveRangeAsync(
            BranchId branchRef,
            TaxDocumentKind documentKind,
            CancellationToken cancellationToken = default);
    }
}
