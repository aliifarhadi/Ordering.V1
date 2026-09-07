using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Resources;
using AeroTech.Ordering.Domain._Shared.ValueObjects;
using NodaTime;

namespace AeroTech.Ordering.Domain.OrderAggregate.ValueObjects
{
    [SpecRef("DD-VO-006")]
    public sealed record FxSnapshot
    {
#pragma warning disable CS8618
        private FxSnapshot()
        {
        }
#pragma warning restore CS8618

        [SpecRef("DD-VO-006")]
        public FxSnapshot(
            CurrencyCode from,
            CurrencyCode to,
            decimal rate,
            string rateSource,
            Instant capturedAtUtc)
        {
            if (string.IsNullOrWhiteSpace(rateSource))
                throw ExceptionFactory.IdentifierIsRequired(nameof(rateSource));

            From = from;
            To = to;
            Rate = rate;
            RateSource = rateSource;
            CapturedAtUtc = capturedAtUtc;
        }

        [SpecRef("DD-VO-006")]
        public CurrencyCode From { get; }

        [SpecRef("DD-VO-006")]
        public CurrencyCode To { get; }

        [SpecRef("DD-VO-006")]
        public decimal Rate { get; }

        [SpecRef("DD-VO-006")]
        public string RateSource { get; }

        [SpecRef("DD-VO-006")]
        public Instant CapturedAtUtc { get; }
    }
}
