using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Enums;
using AeroTech.Ordering.Domain._Shared.Identifiers;
using AeroTech.Ordering.Domain._Shared.ValueObjects;

namespace AeroTech.Ordering.Domain._Shared.Contracts
{
    [SpecRef("DD-AUTHCTX-001")]
    public interface IAuthenticatedContext
    {
        [SpecRef("DD-AUTHCTX-001")]
        SellerId SellerId { get; }

        [SpecRef("CDR-2.3-SalesContext")]
        BranchId BranchId { get; }

        [SpecRef("DD-AUTHCTX-001")]
        ChannelCode Channel { get; }

        [SpecRef("DD-AUTHCTX-001")]
        string? OfficeId { get; }

        [SpecRef("DD-AUTHCTX-001")]
        [Blocked("BL-010", "PointOfSale type and format", "ADR-09")]
        string PointOfSale { get; }

        [SpecRef("DD-AUTHCTX-001")]
        ActorRef Actor { get; }

        [SpecRef("DD-AUTHCTX-001")]
        string CorrelationId { get; }

        [SpecRef("DD-AUTHCTX-001")]
        string? CausationId { get; }
    }
}
