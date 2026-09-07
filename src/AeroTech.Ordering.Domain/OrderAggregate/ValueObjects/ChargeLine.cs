using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Enums;
using AeroTech.Ordering.Domain._Shared.Resources;
using AeroTech.Ordering.Domain._Shared.ValueObjects;

namespace AeroTech.Ordering.Domain.OrderAggregate.ValueObjects
{
    [SpecRef("DD-VO-003")]
    public sealed record ChargeLine
    {
#pragma warning disable CS8618
        private ChargeLine()
        {
        }
#pragma warning restore CS8618

        [SpecRef("DD-VO-003")]
        public ChargeLine(
            ChargeType chargeType,
            string code,
            string? description,
            Money amount,
            bool isRefundable,
            string? taxJurisdiction)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw ExceptionFactory.IdentifierIsRequired(nameof(code));

            if (amount.IsNegative && chargeType is not (ChargeType.Discount or ChargeType.Penalty))
                throw ExceptionFactory.NegativeAmountNotPermittedForChargeType(chargeType);

            ChargeType = chargeType;
            Code = code;
            Description = description;
            Amount = amount;
            IsRefundable = isRefundable;
            TaxJurisdiction = taxJurisdiction;
        }

        [SpecRef("DD-VO-003-01")]
        public ChargeType ChargeType { get; }

        [SpecRef("DD-VO-003-02")]
        public string Code { get; }

        [SpecRef("DD-VO-003-03")]
        public string? Description { get; }

        [SpecRef("DD-VO-003-04")]
        public Money Amount { get; }

        [SpecRef("DD-VO-003-05")]
        public bool IsRefundable { get; }

        [SpecRef("DD-VO-003-06")]
        [Blocked("BL-001", "Required granularity for tax jurisdiction", "O13-C3")]
        public string? TaxJurisdiction { get; }
    }
}
