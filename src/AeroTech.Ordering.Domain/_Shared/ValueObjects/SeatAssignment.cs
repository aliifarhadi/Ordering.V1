using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Resources;
using NodaTime;

namespace AeroTech.Ordering.Domain._Shared.ValueObjects
{
    [SpecRef("DD-VO-022")]
    public sealed record SeatAssignment
    {
#pragma warning disable CS8618
        private SeatAssignment()
        {
        }
#pragma warning restore CS8618

        [SpecRef("DD-VO-022")]
        public SeatAssignment(string seatNumber, Instant assignedAtUtc)
        {
            if (string.IsNullOrWhiteSpace(seatNumber))
                throw ExceptionFactory.IdentifierIsRequired(nameof(seatNumber));

            SeatNumber = seatNumber;
            AssignedAtUtc = assignedAtUtc;
        }

        [SpecRef("DD-VO-022")]
        public string SeatNumber { get; }

        [SpecRef("DD-VO-022")]
        public Instant AssignedAtUtc { get; }
    }
}
