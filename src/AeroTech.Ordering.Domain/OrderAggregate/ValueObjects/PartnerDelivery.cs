using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Enums;
using AeroTech.Ordering.Domain._Shared.ValueObjects;

namespace AeroTech.Ordering.Domain.OrderAggregate.ValueObjects
{
    [SpecRef("DD-VO-020")]
    public sealed record PartnerDelivery
    {
#pragma warning disable CS8618
        private PartnerDelivery()
        {
        }
#pragma warning restore CS8618

        [SpecRef("DD-VO-020")]
        public PartnerDelivery(
            CarrierCode deliveryOwner,
            CarrierCode commercialOwner,
            ResponsibleParty servicingAuthority,
            string? externalOrderReference,
            string? externalItemReference,
            string? partnerStatus,
            OrderItemStatus? partnerStatusMappedTo,
            ControlTransferState controlTransferState,
            ResponsibleParty refundResponsibility,
            ResponsibleParty disruptionResponsibility,
            string? settlementBoundary,
            string? reconciliationReference)
        {
            DeliveryOwner = deliveryOwner;
            CommercialOwner = commercialOwner;
            ServicingAuthority = servicingAuthority;
            ExternalOrderReference = externalOrderReference;
            ExternalItemReference = externalItemReference;
            PartnerStatus = partnerStatus;
            PartnerStatusMappedTo = partnerStatusMappedTo;
            ControlTransferState = controlTransferState;
            RefundResponsibility = refundResponsibility;
            DisruptionResponsibility = disruptionResponsibility;
            SettlementBoundary = settlementBoundary;
            ReconciliationReference = reconciliationReference;
        }

        [SpecRef("DD-VO-020-01")]
        public CarrierCode DeliveryOwner { get; }

        [SpecRef("DD-VO-020-02")]
        public CarrierCode CommercialOwner { get; }

        [SpecRef("DD-VO-020-03")]
        public ResponsibleParty ServicingAuthority { get; }

        [SpecRef("DD-VO-020-04")]
        public string? ExternalOrderReference { get; }

        [SpecRef("DD-VO-020-05")]
        public string? ExternalItemReference { get; }

        [SpecRef("DD-VO-020-06")]
        public string? PartnerStatus { get; }

        [SpecRef("DD-VO-020-07")]
        public OrderItemStatus? PartnerStatusMappedTo { get; }

        [SpecRef("DD-VO-020-08")]
        public ControlTransferState ControlTransferState { get; }

        [SpecRef("DD-VO-020-09")]
        public ResponsibleParty RefundResponsibility { get; }

        [SpecRef("DD-VO-020-10")]
        public ResponsibleParty DisruptionResponsibility { get; }

        [SpecRef("DD-VO-020-11")]
        public string? SettlementBoundary { get; }

        [SpecRef("DD-VO-020-12")]
        public string? ReconciliationReference { get; }
    }
}
