using AeroTech.Ordering.Domain._Shared.Attributes;
using NodaTime;

namespace AeroTech.Ordering.Domain._Shared.ValueObjects
{
    [SpecRef("DD-VO-007")]
    public sealed record FlightSegmentRef
    {
#pragma warning disable CS8618
        private FlightSegmentRef()
        {
        }
#pragma warning restore CS8618

        [SpecRef("DD-VO-007")]
        public FlightSegmentRef(
            CarrierCode marketingCarrier,
            CarrierCode operatingCarrier,
            FlightNumber flightNumber,
            AirportCode origin,
            AirportCode destination,
            Instant departureUtc,
            LocalDate departureLocalDate,
            LocalTime departureLocalTime,
            DateTimeZone originTimeZoneId,
            Instant arrivalUtc,
            LocalDate arrivalLocalDate,
            LocalTime arrivalLocalTime,
            DateTimeZone destinationTimeZoneId)
        {
            MarketingCarrier = marketingCarrier;
            OperatingCarrier = operatingCarrier;
            FlightNumber = flightNumber;
            Origin = origin;
            Destination = destination;
            DepartureUtc = departureUtc;
            DepartureLocalDate = departureLocalDate;
            DepartureLocalTime = departureLocalTime;
            OriginTimeZoneId = originTimeZoneId;
            ArrivalUtc = arrivalUtc;
            ArrivalLocalDate = arrivalLocalDate;
            ArrivalLocalTime = arrivalLocalTime;
            DestinationTimeZoneId = destinationTimeZoneId;
        }

        [SpecRef("DD-VO-007-01")]
        public CarrierCode MarketingCarrier { get; }

        [SpecRef("DD-VO-007-02")]
        public CarrierCode OperatingCarrier { get; }

        [SpecRef("DD-VO-007-03")]
        public FlightNumber FlightNumber { get; }

        [SpecRef("DD-VO-007-04")]
        public AirportCode Origin { get; }

        [SpecRef("DD-VO-007-05")]
        public AirportCode Destination { get; }

        [SpecRef("DD-VO-007-06")]
        public Instant DepartureUtc { get; }

        [SpecRef("DD-VO-007-07")]
        public LocalDate DepartureLocalDate { get; }

        [SpecRef("DD-VO-007-08")]
        public LocalTime DepartureLocalTime { get; }

        [SpecRef("DD-VO-007-09")]
        public DateTimeZone OriginTimeZoneId { get; }

        [SpecRef("DD-VO-007-10")]
        public Instant ArrivalUtc { get; }

        [SpecRef("DD-VO-007-11")]
        public LocalDate ArrivalLocalDate { get; }

        [SpecRef("DD-VO-007-12")]
        public LocalTime ArrivalLocalTime { get; }

        [SpecRef("DD-VO-007-13")]
        public DateTimeZone DestinationTimeZoneId { get; }
    }
}
