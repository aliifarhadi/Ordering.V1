using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Identifiers;
using AeroTech.Ordering.Domain._Shared.Resources;
using AeroTech.Ordering.Domain._Shared.ValueObjects;
using NodaTime;

namespace AeroTech.Ordering.Domain.OrderAggregate.ValueObjects
{
    [SpecRef("DD-VO-005")]
    public sealed record PriceBreakdown
    {
#pragma warning disable CS8618
        private PriceBreakdown()
        {
        }
#pragma warning restore CS8618

        private readonly List<ChargeLine> _lines;
        private readonly List<ValueAllocation> _valueAllocations;

        [SpecRef("DD-VO-005")]
        public PriceBreakdown(
            IReadOnlyList<ChargeLine> lines,
            Money total,
            CurrencyCode currency,
            IReadOnlyList<ValueAllocation> valueAllocations,
            FxSnapshot? fx,
            Instant pricedAtUtc,
            string pricingEngineVersion)
        {
            if (lines.Count == 0)
                throw ExceptionFactory.PriceRequiresAtLeastOneLine();

            if (string.IsNullOrWhiteSpace(pricingEngineVersion))
                throw ExceptionFactory.IdentifierIsRequired(nameof(pricingEngineVersion));

            if (total.Currency != currency || lines.Any(line => line.Amount.Currency != currency))
                throw ExceptionFactory.PriceLinesMustShareCurrency();

            var sum = lines.Aggregate(
                Money.Zero(currency),
                (running, line) => running + line.Amount);

            if (sum != total)
                throw ExceptionFactory.PriceTotalMustEqualSumOfLines();

            if (valueAllocations.Count > 0)
            {
                if (valueAllocations.Any(allocation => allocation.Amount.Currency != currency))
                    throw ExceptionFactory.PriceLinesMustShareCurrency();

                var allocated = valueAllocations.Aggregate(
                    Money.Zero(currency),
                    (running, allocation) => running + allocation.Amount);

                if (allocated != total)
                    throw ExceptionFactory.ValueAllocationMustMatchItemTotal();
            }

            _lines = [.. lines];
            _valueAllocations = [.. valueAllocations];
            Total = total;
            Currency = currency;
            Fx = fx;
            PricedAtUtc = pricedAtUtc;
            PricingEngineVersion = pricingEngineVersion;
        }

        [SpecRef("DD-VO-005-01")]
        public IReadOnlyList<ChargeLine> Lines => _lines.AsReadOnly();

        [SpecRef("DD-VO-005-02")]
        public Money Total { get; }

        [SpecRef("DD-VO-005-03")]
        public CurrencyCode Currency { get; }

        [SpecRef("DD-VO-005-04")]
        public IReadOnlyList<ValueAllocation> ValueAllocations => _valueAllocations.AsReadOnly();

        [SpecRef("DD-VO-005-05")]
        public FxSnapshot? Fx { get; }

        [SpecRef("DD-VO-005-06")]
        public Instant PricedAtUtc { get; }

        [SpecRef("DD-VO-005-07")]
        public string PricingEngineVersion { get; }

        [SpecRef("DD-VO-005-04")]
        public void EnsureAllocationCoversJourneyElements(IReadOnlyList<JourneyElementId> journeyRefs)
        {
            if (journeyRefs.Count <= 1)
                return;

            if (_valueAllocations.Count == 0)
                throw ExceptionFactory.MultiSegmentRequiresValueAllocation();

            foreach (var journeyRef in journeyRefs)
            {
                if (!_valueAllocations.Any(allocation => allocation.JourneyRef == journeyRef))
                    throw ExceptionFactory.MultiSegmentRequiresValueAllocation();
            }
        }
    }
}
