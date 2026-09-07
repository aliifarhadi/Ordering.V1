using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Resources;
using NodaTime;

namespace AeroTech.Ordering.Domain._Shared.ValueObjects
{
    [SpecRef("DD-VO-023")]
    public sealed record DeliveryUnitRef
    {
#pragma warning disable CS8618
        private DeliveryUnitRef()
        {
        }
#pragma warning restore CS8618

        [SpecRef("DD-VO-023")]
        public DeliveryUnitRef(string deliveryRecordNumber, int unitNumber, Instant mirroredAtUtc)
        {
            if (string.IsNullOrWhiteSpace(deliveryRecordNumber))
                throw ExceptionFactory.IdentifierIsRequired(nameof(deliveryRecordNumber));

            DeliveryRecordNumber = deliveryRecordNumber;
            UnitNumber = unitNumber;
            MirroredAtUtc = mirroredAtUtc;
        }

        [SpecRef("DD-VO-023")]
        public string DeliveryRecordNumber { get; }

        [SpecRef("DD-VO-023")]
        public int UnitNumber { get; }

        [SpecRef("DD-VO-023")]
        public Instant MirroredAtUtc { get; }
    }
}
