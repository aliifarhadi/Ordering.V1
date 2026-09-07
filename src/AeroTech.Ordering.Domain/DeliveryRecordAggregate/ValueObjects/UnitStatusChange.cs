using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Enums;
using AeroTech.Ordering.Domain._Shared.Resources;
using NodaTime;

namespace AeroTech.Ordering.Domain.DeliveryRecordAggregate.ValueObjects
{
    [SpecRef("CDR-3.4-UnitStatusChange")]
    public sealed record UnitStatusChange
    {
#pragma warning disable CS8618
        private UnitStatusChange()
        {
        }
#pragma warning restore CS8618

        [SpecRef("CDR-3.4-UnitStatusChange")]
        public UnitStatusChange(
            int seq,
            SegmentDeliveryStatus fromStatus,
            SegmentDeliveryStatus toStatus,
            Instant occurredAtUtc,
            string source)
        {
            if (string.IsNullOrWhiteSpace(source))
                throw ExceptionFactory.IdentifierIsRequired(nameof(source));

            Seq = seq;
            FromStatus = fromStatus;
            ToStatus = toStatus;
            OccurredAtUtc = occurredAtUtc;
            Source = source;
        }

        [SpecRef("CDR-3.4-UnitStatusChange")]
        public int Seq { get; }

        [SpecRef("CDR-3.4-UnitStatusChange")]
        public SegmentDeliveryStatus FromStatus { get; }

        [SpecRef("CDR-3.4-UnitStatusChange")]
        public SegmentDeliveryStatus ToStatus { get; }

        [SpecRef("CDR-3.4-UnitStatusChange")]
        public Instant OccurredAtUtc { get; }

        [SpecRef("CDR-3.4-UnitStatusChange")]
        public string Source { get; }
    }
}
