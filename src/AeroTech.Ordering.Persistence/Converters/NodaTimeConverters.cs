using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NodaTime;
using NodaTime.Text;

namespace AeroTech.Ordering.Persistence.Converters
{
    public sealed class InstantConverter : ValueConverter<Instant, DateTime>
    {
        public InstantConverter()
            : base(
                instant => instant.ToDateTimeUtc(),
                value => Instant.FromDateTimeUtc(DateTime.SpecifyKind(value, DateTimeKind.Utc)))
        {
        }
    }

    public sealed class NullableInstantConverter : ValueConverter<Instant?, DateTime?>
    {
        public NullableInstantConverter()
            : base(
                instant => instant == null ? null : instant.Value.ToDateTimeUtc(),
                value => value == null
                    ? null
                    : Instant.FromDateTimeUtc(DateTime.SpecifyKind(value.Value, DateTimeKind.Utc)))
        {
        }
    }

    public sealed class LocalDateConverter : ValueConverter<LocalDate, DateOnly>
    {
        public LocalDateConverter()
            : base(
                date => new DateOnly(date.Year, date.Month, date.Day),
                value => new LocalDate(value.Year, value.Month, value.Day))
        {
        }
    }

    public sealed class NullableLocalDateConverter : ValueConverter<LocalDate?, DateOnly?>
    {
        public NullableLocalDateConverter()
            : base(
                date => date == null ? null : new DateOnly(date.Value.Year, date.Value.Month, date.Value.Day),
                value => value == null ? null : new LocalDate(value.Value.Year, value.Value.Month, value.Value.Day))
        {
        }
    }

    public sealed class LocalTimeConverter : ValueConverter<LocalTime, TimeOnly>
    {
        public LocalTimeConverter()
            : base(
                time => new TimeOnly(time.Hour, time.Minute, time.Second),
                value => new LocalTime(value.Hour, value.Minute, value.Second))
        {
        }
    }

    public sealed class DateTimeZoneConverter : ValueConverter<DateTimeZone, string>
    {
        public DateTimeZoneConverter()
            : base(
                zone => zone.Id,
                value => DateTimeZoneProviders.Tzdb[value])
        {
        }
    }
}
