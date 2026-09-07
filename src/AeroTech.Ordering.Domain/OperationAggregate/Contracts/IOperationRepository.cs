using AeroTech.Framework.Core.Domain.Repository;
using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Enums;
using AeroTech.Ordering.Domain._Shared.Identifiers;

namespace AeroTech.Ordering.Domain.OperationAggregate.Contracts
{
    [SpecRef("DD-AGG-002")]
    public interface IOperationRepository : IRepository<Operation, OperationId>
    {
        [SpecRef("DD-AGG-002-16")]
        Task<Operation?> GetByIdempotencyKeyAsync(string idempotencyKey, CancellationToken cancellationToken = default);

        [SpecRef("DD-AGG-002-17")]
        Task<IReadOnlyList<OrderItemId>> GetItemsCoveredByOpenOperationAsync(
            OrderId orderRef,
            OperationType type,
            CancellationToken cancellationToken = default);
    }
}
