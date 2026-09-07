using AeroTech.Framework.Core.Domain.Repository;
using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Identifiers;

namespace AeroTech.Ordering.Domain.GroupBookingAggregate.Contracts
{
    [SpecRef("CDR-2.2.1")]
    public interface IGroupBookingRepository : IRepository<GroupBooking, GroupBookingId>
    {
        [SpecRef("CDR-2.2.1")]
        Task<GroupBooking?> GetByReferenceAsync(
            GroupReference reference,
            CancellationToken cancellationToken = default);
    }
}
