using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Contracts;
using AeroTech.Ordering.Domain._Shared.Enums;
using AeroTech.Ordering.Domain._Shared.Identifiers;
using NodaTime;

namespace AeroTech.Ordering.Domain._Shared.ValueObjects
{
    [SpecRef("DD-VO-016")]
    public sealed record SalesContext
    {
#pragma warning disable CS8618
        private SalesContext()
        {
        }
#pragma warning restore CS8618

        private SalesContext(
            SellerId sellerId,
            BranchId branchId,
            ChannelCode channel,
            string? officeId,
            string pointOfSale,
            CurrencyCode saleCurrency,
            Instant soldAtUtc,
            ActorRef createdBy)
        {
            SellerId = sellerId;
            BranchId = branchId;
            Channel = channel;
            OfficeId = officeId;
            PointOfSale = pointOfSale;
            SaleCurrency = saleCurrency;
            SoldAtUtc = soldAtUtc;
            CreatedBy = createdBy;
        }

        [SpecRef("DD-VO-016")]
        public static SalesContext FromAuthenticatedContext(
            IAuthenticatedContext context,
            CurrencyCode saleCurrency,
            Instant soldAtUtc) =>
            new(
                sellerId: context.SellerId,
                branchId: context.BranchId,
                channel: context.Channel,
                officeId: context.OfficeId,
                pointOfSale: context.PointOfSale,
                saleCurrency: saleCurrency,
                soldAtUtc: soldAtUtc,
                createdBy: context.Actor);

        [SpecRef("DD-VO-016-01")]
        public SellerId SellerId { get; }

        [SpecRef("CDR-2.3-SalesContext")]
        public BranchId BranchId { get; }

        [SpecRef("DD-VO-016-02")]
        public ChannelCode Channel { get; }

        [SpecRef("DD-VO-016-03")]
        public string? OfficeId { get; }

        [SpecRef("DD-VO-016-04")]
        [Blocked("BL-010", "PointOfSale type and format", "ADR-09")]
        public string PointOfSale { get; }

        [SpecRef("DD-VO-016-05")]
        public CurrencyCode SaleCurrency { get; }

        [SpecRef("DD-VO-016-06")]
        public Instant SoldAtUtc { get; }

        [SpecRef("DD-VO-016-07")]
        public ActorRef CreatedBy { get; }
    }
}
