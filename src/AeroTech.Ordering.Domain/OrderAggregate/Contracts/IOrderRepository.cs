using AeroTech.Framework.Core.Domain.Repository;
using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Identifiers;

namespace AeroTech.Ordering.Domain.OrderAggregate.Contracts
{
    [SpecRef("DD-AGG-001")]
    public interface IOrderRepository : IRepository<Order, OrderId>
    {
        [SpecRef("DD-AGG-001-02")]
        Task<Order?> GetByReferenceAsync(OrderReference reference, CancellationToken cancellationToken = default);
    }
}
