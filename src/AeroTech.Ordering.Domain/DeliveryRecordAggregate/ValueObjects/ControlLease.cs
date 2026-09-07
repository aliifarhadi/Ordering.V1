using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Resources;
using NodaTime;

namespace AeroTech.Ordering.Domain.DeliveryRecordAggregate.ValueObjects
{
    [SpecRef("CDR-3.4-ControlLease")]
    public sealed record ControlLease
    {
#pragma warning disable CS8618
        private ControlLease()
        {
        }
#pragma warning restore CS8618

        [SpecRef("CDR-3.4-ControlLease")]
        public ControlLease(string holder, Instant acquiredAtUtc, Instant expiresAtUtc)
        {
            if (string.IsNullOrWhiteSpace(holder))
                throw ExceptionFactory.IdentifierIsRequired(nameof(holder));

            Holder = holder;
            AcquiredAtUtc = acquiredAtUtc;
            ExpiresAtUtc = expiresAtUtc;
        }

        [SpecRef("CDR-3.4-ControlLease")]
        public string Holder { get; }

        [SpecRef("CDR-3.4-ControlLease")]
        public Instant AcquiredAtUtc { get; }

        [SpecRef("CDR-3.4-ControlLease")]
        public Instant ExpiresAtUtc { get; }
    }
}
