using AeroTech.Ordering.Domain.GroupBookingAggregate;
using AeroTech.Ordering.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroTech.Ordering.Persistence.GroupBookingAggregate
{
    public sealed class GroupBookingConfiguration : IEntityTypeConfiguration<GroupBooking>
    {
        public void Configure(EntityTypeBuilder<GroupBooking> builder)
        {
            builder.ToTable("GroupBookings", OrderingDbContext.OrderingSchema);

            builder.HasKey(group => group.Id);

            builder.Property(group => group.Id)
                .HasConversion<GroupBookingIdConverter>()
                .ValueGeneratedNever();

            builder.Property(group => group.Reference)
                .HasConversion<GroupReferenceConverter>()
                .HasMaxLength(64)
                .IsRequired();

            builder.HasIndex(group => group.Reference).IsUnique();

            builder.Property(group => group.ClosedAtUtc)
                .HasConversion<NullableInstantConverter>()
                .HasColumnType("datetime2(7)");

            builder.Property(group => group.IsSuspended).IsRequired();
            builder.Property(group => group.BlocksConfirmed).IsRequired();
            builder.Property(group => group.RowVersion).IsRowVersion();

            builder.Ignore(group => group.Status);

            builder.OwnsOne(group => group.SalesContext, salesContext =>
            {
                salesContext.Property(context => context.SellerId)
                    .HasConversion<SellerIdConverter>()
                    .HasColumnName("SellerId").HasMaxLength(64).IsRequired();

                salesContext.Property(context => context.BranchId)
                    .HasConversion<BranchIdConverter>()
                    .HasColumnName("BranchId").HasMaxLength(64).IsRequired();

                salesContext.Property(context => context.Channel)
                    .HasConversion<string>()
                    .HasColumnName("ChannelCode").HasMaxLength(32).IsRequired();

                salesContext.Property(context => context.OfficeId)
                    .HasColumnName("OfficeId").HasMaxLength(64);

                salesContext.Property(context => context.PointOfSale)
                    .HasColumnName("PointOfSale").HasMaxLength(32).IsRequired();

                salesContext.Property(context => context.SaleCurrency)
                    .HasConversion<CurrencyCodeConverter>()
                    .HasColumnName("SaleCurrency").HasMaxLength(3).IsRequired();

                salesContext.Property(context => context.SoldAtUtc)
                    .HasConversion<InstantConverter>()
                    .HasColumnName("SoldAtUtc").HasColumnType("datetime2(7)").IsRequired();

                salesContext.OwnsOne(context => context.CreatedBy, actor =>
                {
                    actor.Property(reference => reference.ActorType)
                        .HasConversion<string>()
                        .HasColumnName("CreatedByActorType").HasMaxLength(32).IsRequired();
                    actor.Property(reference => reference.UserId)
                        .HasColumnName("CreatedByUserId").HasMaxLength(128).IsRequired();
                    actor.Property(reference => reference.SellerId)
                        .HasConversion<NullableSellerIdConverter>()
                        .HasColumnName("CreatedBySellerId").HasMaxLength(64);
                    actor.Property(reference => reference.BranchId)
                        .HasConversion<NullableBranchIdConverter>()
                        .HasColumnName("CreatedByBranchId").HasMaxLength(64);
                    actor.Property(reference => reference.OfficeId)
                        .HasColumnName("CreatedByOfficeId").HasMaxLength(64);
                    actor.Property(reference => reference.HasAirlineOverride)
                        .HasColumnName("CreatedByHasAirlineOverride").IsRequired();
                });
            });

            builder.OwnsMany(group => group.Blocks, block =>
            {
                block.ToTable("GroupSeatBlocks", OrderingDbContext.OrderingSchema);
                block.WithOwner().HasForeignKey("GroupBookingId");
                block.HasKey("GroupBookingId", "Id");

                block.Property(entity => entity.Id)
                    .HasConversion<BlockIdConverter>()
                    .HasColumnName("BlockId").HasMaxLength(32);

                block.Property(entity => entity.Cabin)
                    .HasConversion<CabinCodeConverter>().HasMaxLength(8).IsRequired();
                block.Property(entity => entity.Rbd)
                    .HasConversion<RbdCodeConverter>().HasMaxLength(8).IsRequired();

                block.Property(entity => entity.SeatsHeld).IsRequired();
                block.Property(entity => entity.SeatsAllocated).IsRequired();
                block.Property(entity => entity.SeatsReleased).IsRequired();
                block.Property(entity => entity.InventoryBlockRef).HasMaxLength(128);

                block.Ignore(entity => entity.SeatsAvailable);
                block.Ignore(entity => entity.LastUpdateTime);
                block.Ignore(entity => entity.LastUpdatedBy);

                block.OwnsOne(entity => entity.Flight, flight =>
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
            });

            builder.OwnsMany(group => group.Slots, slot =>
            {
                slot.ToTable("GroupNameSlots", OrderingDbContext.OrderingSchema);
                slot.WithOwner().HasForeignKey("GroupBookingId");
                slot.HasKey("GroupBookingId", "Id");

                slot.Property(entity => entity.Id)
                    .HasConversion<SlotIdConverter>()
                    .HasColumnName("SlotId").HasMaxLength(32);

                slot.Property(entity => entity.Status)
                    .HasConversion<string>().HasMaxLength(32).IsRequired();

                slot.Property(entity => entity.Ptc)
                    .HasConversion<string>().HasMaxLength(8);

                slot.Property(entity => entity.OrderRef)
                    .HasConversion<NullableOrderIdConverter>()
                    .HasColumnName("OrderId");

                slot.Ignore(entity => entity.LastUpdateTime);
                slot.Ignore(entity => entity.LastUpdatedBy);

                slot.OwnsOne(entity => entity.Name, name =>
                {
                    name.Property(value => value.Given).HasColumnName("Given").HasMaxLength(128);
                    name.Property(value => value.Surname).HasColumnName("Surname").HasMaxLength(128);
                    name.Property(value => value.Title).HasColumnName("Title").HasMaxLength(32);
                    name.Property(value => value.GivenLocal).HasColumnName("GivenLocal").HasMaxLength(128);
                    name.Property(value => value.SurnameLocal).HasColumnName("SurnameLocal").HasMaxLength(128);
                });
            });

            builder.OwnsMany(group => group.Payments, payment =>
            {
                payment.ToTable("GroupPaymentRecords", OrderingDbContext.OrderingSchema);
                payment.WithOwner().HasForeignKey("GroupBookingId");
                payment.HasKey("GroupBookingId", "Id");

                payment.Property(entity => entity.Id)
                    .HasConversion<PaymentRecordIdConverter>()
                    .HasColumnName("PaymentRecordId").HasMaxLength(32);

                payment.Property(entity => entity.Fop)
                    .HasConversion<string>().HasColumnName("FormOfPayment").HasMaxLength(32).IsRequired();
                payment.Property(entity => entity.Status)
                    .HasConversion<string>().HasMaxLength(32).IsRequired();
                payment.Property(entity => entity.RequestRef)
                    .HasConversion<PaymentRequestRefConverter>()
                    .HasColumnName("PaymentRequestRef").HasMaxLength(128);
                payment.Property(entity => entity.PayerRef).HasMaxLength(128);
                payment.Property(entity => entity.RecordedAtUtc)
                    .HasConversion<InstantConverter>().HasColumnType("datetime2(7)").IsRequired();
                payment.Property(entity => entity.IsUnderDispute).IsRequired();

                payment.Ignore(entity => entity.IsSettled);
                payment.Ignore(entity => entity.LastUpdateTime);
                payment.Ignore(entity => entity.LastUpdatedBy);

                payment.OwnsOne(entity => entity.Amount, money =>
                {
                    money.Property(amount => amount.Amount)
                        .HasColumnName("Amount").HasColumnType("decimal(19,4)").IsRequired();
                    money.Property(amount => amount.Currency)
                        .HasConversion<CurrencyCodeConverter>()
                        .HasColumnName("Currency").HasMaxLength(3).IsRequired();
                });

                payment.OwnsOne(entity => entity.Fx, fx =>
                {
                    fx.Property(value => value.From)
                        .HasConversion<CurrencyCodeConverter>().HasColumnName("FxFrom").HasMaxLength(3);
                    fx.Property(value => value.To)
                        .HasConversion<CurrencyCodeConverter>().HasColumnName("FxTo").HasMaxLength(3);
                    fx.Property(value => value.Rate)
                        .HasColumnName("FxRate").HasColumnType("decimal(19,8)");
                    fx.Property(value => value.RateSource)
                        .HasColumnName("FxSource").HasMaxLength(64);
                    fx.Property(value => value.CapturedAtUtc)
                        .HasConversion<InstantConverter>()
                        .HasColumnName("FxCapturedAtUtc").HasColumnType("datetime2(7)");
                });

                payment.OwnsMany(entity => entity.Allocations, allocation =>
                {
                    allocation.ToTable("GroupPaymentAllocations", OrderingDbContext.OrderingSchema);
                    allocation.WithOwner().HasForeignKey("GroupBookingId", "PaymentRecordId");

                    allocation.Property(value => value.ItemRef)
                        .HasConversion<OrderItemIdConverter>()
                        .HasColumnName("OrderItemId").HasMaxLength(32);

                    allocation.HasKey("GroupBookingId", "PaymentRecordId", "ItemRef");

                    allocation.OwnsOne(value => value.Amount, money =>
                    {
                        money.Property(amount => amount.Amount)
                            .HasColumnName("Amount").HasColumnType("decimal(19,4)").IsRequired();
                        money.Property(amount => amount.Currency)
                            .HasConversion<CurrencyCodeConverter>()
                            .HasColumnName("Currency").HasMaxLength(3).IsRequired();
                    });
                });
            });

            builder.OwnsMany(group => group.TimeLimits, timeLimit =>
            {
                timeLimit.ToTable("GroupTimeLimits", OrderingDbContext.OrderingSchema);
                timeLimit.WithOwner().HasForeignKey("GroupBookingId");
                timeLimit.HasKey("GroupBookingId", "Id");

                timeLimit.Property(entity => entity.Id)
                    .HasConversion<TimeLimitIdConverter>()
                    .HasColumnName("TimeLimitId").HasMaxLength(32);

                timeLimit.Property(entity => entity.Type)
                    .HasConversion<string>().HasMaxLength(32).IsRequired();
                timeLimit.Property(entity => entity.Status)
                    .HasConversion<string>().HasMaxLength(32).IsRequired();
                timeLimit.Property(entity => entity.DueAtUtc)
                    .HasConversion<InstantConverter>().HasColumnType("datetime2(7)").IsRequired();
                timeLimit.Property(entity => entity.ExtensionCount).IsRequired();

                timeLimit.Ignore(entity => entity.IsActive);
                timeLimit.Ignore(entity => entity.LastUpdateTime);
                timeLimit.Ignore(entity => entity.LastUpdatedBy);

                timeLimit.HasIndex(entity => new { entity.Status, entity.DueAtUtc })
                    .HasFilter("[Status] = 'Active'");

                timeLimit.Property(entity => entity.AppliesTo)
                    .HasConversion(new OrderItemIdListConverter(), new OrderItemIdListComparer())
                    .HasColumnName("AppliesTo").HasMaxLength(512);
            });

            builder.Property(group => group.SpawnedOrders)
                .HasConversion(new OrderIdListConverter(), new OrderIdListComparer())
                .HasColumnName("SpawnedOrders")
                .HasColumnType("nvarchar(max)");
        }
    }
}
