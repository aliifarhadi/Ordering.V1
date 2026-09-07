using AeroTech.Ordering.Domain.DeliveryRecordAggregate;
using AeroTech.Ordering.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroTech.Ordering.Persistence.DeliveryRecordAggregate
{
    public sealed class DeliveryRecordConfiguration : IEntityTypeConfiguration<DeliveryRecord>
    {
        public void Configure(EntityTypeBuilder<DeliveryRecord> builder)
        {
            builder.ToTable("DeliveryRecords", OrderingDbContext.DeliverySchema);

            builder.HasKey(record => record.Id);

            builder.Property(record => record.Id)
                .HasConversion<DeliveryRecordNumberConverter>()
                .HasColumnName("DeliveryRecordNumber")
                .HasMaxLength(64)
                .ValueGeneratedNever();

            builder.Property(record => record.OrderRef)
                .HasConversion<OrderIdConverter>()
                .HasColumnName("OrderId")
                .IsRequired();

            builder.Property(record => record.TravelerRef)
                .HasConversion<TravelerIdConverter>()
                .HasColumnName("TravelerId").HasMaxLength(32).IsRequired();

            builder.Property(record => record.BranchRef)
                .HasConversion<BranchIdConverter>()
                .HasColumnName("BranchId").HasMaxLength(64).IsRequired();

            builder.Property(record => record.IssuedAtUtc)
                .HasConversion<InstantConverter>()
                .HasColumnType("datetime2(7)").IsRequired();

            builder.Property(record => record.VoidWindowHours).IsRequired();
            builder.Property(record => record.RowVersion).IsRowVersion();

            builder.Ignore(record => record.Number);
            builder.Ignore(record => record.Status);

            builder.HasIndex(record => record.OrderRef);

            builder.OwnsOne(record => record.TravelerSnapshot, snapshot =>
            {
                snapshot.Property(value => value.Ptc)
                    .HasConversion<string>().HasColumnName("Ptc").HasMaxLength(8).IsRequired();

                snapshot.Property(value => value.DateOfBirth)
                    .HasConversion<NullableLocalDateConverter>()
                    .HasColumnName("DateOfBirth").HasColumnType("date");

                snapshot.OwnsOne(value => value.Name, name =>
                {
                    name.Property(value => value.Given).HasColumnName("Given").HasMaxLength(128).IsRequired();
                    name.Property(value => value.Surname).HasColumnName("Surname").HasMaxLength(128).IsRequired();
                    name.Property(value => value.Title).HasColumnName("Title").HasMaxLength(32);
                    name.Property(value => value.GivenLocal).HasColumnName("GivenLocal").HasMaxLength(128);
                    name.Property(value => value.SurnameLocal).HasColumnName("SurnameLocal").HasMaxLength(128);
                });

                snapshot.OwnsOne(value => value.PrimaryDocument, document =>
                {
                    document.Property(value => value.DocType)
                        .HasConversion<string>()
                        .HasColumnName("DocumentType").HasMaxLength(32);
                    document.Property(value => value.Number)
                        .HasColumnName("DocumentNumber").HasMaxLength(128);
                    document.Property(value => value.IssuingCountry)
                        .HasColumnName("DocumentIssuingCountry").HasMaxLength(8);
                    document.Property(value => value.Nationality)
                        .HasColumnName("DocumentNationality").HasMaxLength(8);
                    document.Property(value => value.Expiry)
                        .HasConversion<NullableLocalDateConverter>()
                        .HasColumnName("DocumentExpiry").HasColumnType("date");
                });
            });

            builder.OwnsMany(record => record.SegmentUnits, unit =>
            {
                unit.ToTable("SegmentDeliveries", OrderingDbContext.DeliverySchema);
                unit.WithOwner().HasForeignKey("DeliveryRecordNumber");
                unit.HasKey("DeliveryRecordNumber", "UnitNumber");

                unit.Property(entity => entity.UnitNumber).ValueGeneratedNever();

                unit.Property(entity => entity.OrderItemRef)
                    .HasConversion<OrderItemIdConverter>()
                    .HasColumnName("OrderItemId").HasMaxLength(32).IsRequired();

                unit.Property(entity => entity.JourneyRef)
                    .HasConversion<JourneyElementIdConverter>()
                    .HasColumnName("JourneyElementId").HasMaxLength(32).IsRequired();

                unit.Property(entity => entity.Status)
                    .HasConversion<string>().HasMaxLength(32).IsRequired();

                unit.Property(entity => entity.ControlHolder).HasMaxLength(32);

                unit.Property(entity => entity.ControlAcquiredAtUtc)
                    .HasConversion<NullableInstantConverter>().HasColumnType("datetime2(7)");

                unit.Property(entity => entity.ControlLeaseExpiresAtUtc)
                    .HasConversion<NullableInstantConverter>().HasColumnType("datetime2(7)");

                unit.Ignore(entity => entity.IsUnderExternalControl);
                unit.Ignore(entity => entity.IsTerminal);
                unit.Ignore(entity => entity.IsConsumed);

                unit.OwnsOne(entity => entity.UnitValue, money =>
                {
                    money.Property(amount => amount.Amount)
                        .HasColumnName("UnitValue").HasColumnType("decimal(19,4)").IsRequired();
                    money.Property(amount => amount.Currency)
                        .HasConversion<CurrencyCodeConverter>()
                        .HasColumnName("Currency").HasMaxLength(3).IsRequired();
                });

                unit.OwnsOne(entity => entity.Flight, flight =>
                {
                    flight.Property(value => value.MarketingCarrier)
                        .HasConversion<CarrierCodeConverter>()
                        .HasColumnName("MarketingCarrier").HasMaxLength(8).IsRequired();
                    flight.Property(value => value.OperatingCarrier)
                        .HasConversion<CarrierCodeConverter>()
                        .HasColumnName("OperatingCarrier").HasMaxLength(8).IsRequired();
                    flight.Property(value => value.FlightNumber)
                        .HasConversion<FlightNumberConverter>()
                        .HasColumnName("FlightNumber").HasMaxLength(16).IsRequired();
                    flight.Property(value => value.Origin)
                        .HasConversion<AirportCodeConverter>()
                        .HasColumnName("Origin").HasMaxLength(8).IsRequired();
                    flight.Property(value => value.Destination)
                        .HasConversion<AirportCodeConverter>()
                        .HasColumnName("Destination").HasMaxLength(8).IsRequired();
                    flight.Property(value => value.DepartureUtc)
                        .HasConversion<InstantConverter>()
                        .HasColumnName("DepartureUtc").HasColumnType("datetime2(7)").IsRequired();
                    flight.Property(value => value.DepartureLocalDate)
                        .HasConversion<LocalDateConverter>()
                        .HasColumnName("DepartureLocalDate").HasColumnType("date").IsRequired();
                    flight.Property(value => value.DepartureLocalTime)
                        .HasConversion<LocalTimeConverter>()
                        .HasColumnName("DepartureLocalTime").HasColumnType("time(0)").IsRequired();
                    flight.Property(value => value.OriginTimeZoneId)
                        .HasConversion<DateTimeZoneConverter>()
                        .HasColumnName("OriginTimeZoneId").HasMaxLength(64).IsRequired();
                    flight.Property(value => value.ArrivalUtc)
                        .HasConversion<InstantConverter>()
                        .HasColumnName("ArrivalUtc").HasColumnType("datetime2(7)").IsRequired();
                    flight.Property(value => value.ArrivalLocalDate)
                        .HasConversion<LocalDateConverter>()
                        .HasColumnName("ArrivalLocalDate").HasColumnType("date").IsRequired();
                    flight.Property(value => value.ArrivalLocalTime)
                        .HasConversion<LocalTimeConverter>()
                        .HasColumnName("ArrivalLocalTime").HasColumnType("time(0)").IsRequired();
                    flight.Property(value => value.DestinationTimeZoneId)
                        .HasConversion<DateTimeZoneConverter>()
                        .HasColumnName("DestinationTimeZoneId").HasMaxLength(64).IsRequired();

                    flight.HasIndex("MarketingCarrier", "FlightNumber", "DepartureLocalDate");
                });

                unit.OwnsMany(entity => entity.StatusHistory, change =>
                {
                    change.ToTable("SegmentDeliveryStatusHistory", OrderingDbContext.DeliverySchema);
                    change.WithOwner().HasForeignKey("DeliveryRecordNumber", "UnitNumber");
                    change.HasKey("DeliveryRecordNumber", "UnitNumber", "Seq");

                    change.Property(value => value.Seq).ValueGeneratedNever();
                    change.Property(value => value.FromStatus)
                        .HasConversion<string>().HasMaxLength(32).IsRequired();
                    change.Property(value => value.ToStatus)
                        .HasConversion<string>().HasMaxLength(32).IsRequired();
                    change.Property(value => value.OccurredAtUtc)
                        .HasConversion<InstantConverter>().HasColumnType("datetime2(7)").IsRequired();
                    change.Property(value => value.Source).HasMaxLength(64).IsRequired();
                });
            });

            builder.OwnsMany(record => record.ServiceUnits, unit =>
            {
                unit.ToTable("ServiceDeliveries", OrderingDbContext.DeliverySchema);
                unit.WithOwner().HasForeignKey("DeliveryRecordNumber");
                unit.HasKey("DeliveryRecordNumber", "UnitNumber");

                unit.Property(entity => entity.UnitNumber).ValueGeneratedNever();

                unit.Property(entity => entity.OrderItemRef)
                    .HasConversion<OrderItemIdConverter>()
                    .HasColumnName("OrderItemId").HasMaxLength(32).IsRequired();

                unit.Property(entity => entity.ServiceCode).HasMaxLength(64).IsRequired();
                unit.Property(entity => entity.Description).HasMaxLength(256).IsRequired();

                unit.Property(entity => entity.Status)
                    .HasConversion<string>().HasMaxLength(32).IsRequired();

                unit.Property(entity => entity.AssociatedSegmentUnit);

                unit.Ignore(entity => entity.IsOpen);
                unit.Ignore(entity => entity.IsTerminal);

                unit.OwnsOne(entity => entity.UnitValue, money =>
                {
                    money.Property(amount => amount.Amount)
                        .HasColumnName("UnitValue").HasColumnType("decimal(19,4)").IsRequired();
                    money.Property(amount => amount.Currency)
                        .HasConversion<CurrencyCodeConverter>()
                        .HasColumnName("Currency").HasMaxLength(3).IsRequired();
                });
            });
        }
    }
}
