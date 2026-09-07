using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Resources;
using AeroTech.Ordering.Domain._Shared.ValueObjects;

namespace AeroTech.Ordering.Domain.TaxDocumentAggregate.Entities
{
    [SpecRef("CDR-3.2.2")]
    public sealed class TaxLine
    {
        private TaxLine()
        {
        }

        [SpecRef("CDR-3.2.2")]
        internal TaxLine(
            int seq,
            string lineType,
            string code,
            string description,
            Money netAmount,
            Money taxAmount,
            decimal? taxRate,
            string? taxJurisdiction)
        {
            if (string.IsNullOrWhiteSpace(lineType))
                throw ExceptionFactory.IdentifierIsRequired(nameof(lineType));

            if (string.IsNullOrWhiteSpace(code))
                throw ExceptionFactory.IdentifierIsRequired(nameof(code));

            Seq = seq;
            LineType = lineType;
            Code = code;
            Description = description;
            NetAmount = netAmount;
            TaxAmount = taxAmount;
            TaxRate = taxRate;
            TaxJurisdiction = taxJurisdiction;
        }

        [SpecRef("CDR-3.2.2")]
        public int Seq { get; private set; }

        [SpecRef("CDR-3.2.2")]
        public string LineType { get; private set; } = null!;

        [SpecRef("CDR-3.2.2")]
        public string Code { get; private set; } = null!;

        [SpecRef("CDR-3.2.2")]
        public string Description { get; private set; } = null!;

        [SpecRef("CDR-3.2.2")]
        public Money NetAmount { get; private set; } = null!;

        [SpecRef("CDR-3.2.2")]
        public Money TaxAmount { get; private set; } = null!;

        [SpecRef("CDR-3.2.2")]
        public decimal? TaxRate { get; private set; }

        [SpecRef("CDR-3.2.2")]
        [Blocked("BL-001", "Required granularity for tax jurisdiction", "O13-C3")]
        public string? TaxJurisdiction { get; private set; }
    }
}
