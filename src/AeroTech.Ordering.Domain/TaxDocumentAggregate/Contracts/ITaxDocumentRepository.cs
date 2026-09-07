using AeroTech.Framework.Core.Domain.Repository;
using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Enums;
using AeroTech.Ordering.Domain._Shared.Identifiers;

namespace AeroTech.Ordering.Domain.TaxDocumentAggregate.Contracts
{
    [SpecRef("CDR-3.2.1")]
    public interface ITaxDocumentRepository : IRepository<TaxDocument, DocumentNumber>
    {
        [SpecRef("CDR-3.2.1")]
        Task<IReadOnlyList<TaxDocument>> GetByOrderAsync(
            OrderId orderRef,
            CancellationToken cancellationToken = default);

        [SpecRef("CDR-3.2.5")]
        Task<IReadOnlyList<TaxDocument>> GetBySubmissionStatusAsync(
            SubmissionStatus submissionStatus,
            int maxResults,
            CancellationToken cancellationToken = default);
    }
}
