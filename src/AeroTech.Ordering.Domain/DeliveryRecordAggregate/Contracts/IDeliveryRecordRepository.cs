using AeroTech.Framework.Core.Domain.Repository;
using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Identifiers;

namespace AeroTech.Ordering.Domain.DeliveryRecordAggregate.Contracts
{
    [SpecRef("CDR-3.1.1")]
    public interface IDeliveryRecordRepository : IRepository<DeliveryRecord, DeliveryRecordNumber>
    {
        [SpecRef("CDR-3.1.1")]
        Task<IReadOnlyList<DeliveryRecord>> GetByOrderAsync(
            OrderId orderRef,
            CancellationToken cancellationToken = default);
    }
}
