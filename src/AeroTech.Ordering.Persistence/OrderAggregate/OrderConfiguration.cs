using AeroTech.Ordering.Domain.OrderAggregate;
using AeroTech.Ordering.Domain.OrderAggregate.Entities;
using AeroTech.Ordering.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroTech.Ordering.Persistence.OrderAggregate
{
    public sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders", OrderingDbContext.OrderingSchema);

            builder.HasKey(order => order.Id);

            builder.Property(order => order.Id)
                .HasConversion<OrderIdConverter>()
                .ValueGeneratedNever();

            builder.Property(order => order.Reference)
                .HasConversion<OrderReferenceConverter>()
                .HasMaxLength(64)
                .IsRequired();

            builder.HasIndex(order => order.Reference).IsUnique();

            builder.Property(order => order.GroupRef)
                .HasConversion<NullableGroupBookingIdConverter>()
                .HasColumnName("GroupBookingId");

            builder.Property(order => order.AggregateVersion).IsConcurrencyToken(false);

            builder.Property(order => order.ClosedAtUtc)
                .HasConversion<NullableInstantConverter>()
                .HasColumnType("datetime2(7)");

            builder.Property(order => order.IsSuspended).IsRequired();
            builder.Property(order => order.IsUnderLegalHold).IsRequired();
            builder.Property(order => order.RequiresReview).IsRequired();

            builder.Property(order => order.RowVersion).IsRowVersion();

            builder.Ignore(order => order.Status);
            builder.Ignore(order => order.Totals);

            ConfigureSalesContext(builder);
            ConfigureTravelers(builder);
            ConfigureJourneyElements(builder);
            ConfigureSegments(builder);
            ConfigureItems(builder);
            ConfigurePayments(builder);
            ConfigureTimeLimits(builder);
            ConfigureContacts(builder);
            ConfigureHistory(builder);
        }

        private static void ConfigureSalesContext(EntityTypeBuilder<Order> builder)
        {
            builder.OwnsOne(order => order.SalesContext, salesContext =>
            {
                salesContext.Property(context => context.SellerId)
                    .HasConversion<SellerIdConverter>()
                    .HasColumnName("SellerId")
                    .HasMaxLength(64)
                    .IsRequired();

                salesContext.Property(context => context.BranchId)
                    .HasConversion<BranchIdConverter>()
                    .HasColumnName("BranchId")
                    .HasMaxLength(64)
                    .IsRequired();

                salesContext.Property(context => context.Channel)
                    .HasConversion<string>()
                    .HasColumnName("ChannelCode")
                    .HasMaxLength(32)
                    .IsRequired();

                salesContext.Property(context => context.OfficeId)
                    .HasColumnName("OfficeId")
                    .HasMaxLength(64);

                salesContext.Property(context => context.PointOfSale)
                    .HasColumnName("PointOfSale")
                    .HasMaxLength(32)
                    .IsRequired();

                salesContext.Property(context => context.SaleCurrency)
                    .HasConversion<CurrencyCodeConverter>()
                    .HasColumnName("SaleCurrency")
                    .HasMaxLength(3)
                    .IsRequired();

                salesContext.Property(context => context.SoldAtUtc)
                    .HasConversion<InstantConverter>()
                    .HasColumnName("SoldAtUtc")
                    .HasColumnType("datetime2(7)")
                    .IsRequired();

                salesContext.OwnsOne(context => context.CreatedBy, actor =>
                {
                    actor.Property(reference => reference.ActorType)
                        .HasConversion<string>()
                        .HasColumnName("CreatedByActorType")
                        .HasMaxLength(32)
                        .IsRequired();

                    actor.Property(reference => reference.UserId)
                        .HasColumnName("CreatedByUserId")
                        .HasMaxLength(128)
                        .IsRequired();

                    actor.Property(reference => reference.SellerId)
                        .HasConversion<NullableSellerIdConverter>()
                        .HasColumnName("CreatedBySellerId")
                        .HasMaxLength(64);

                    actor.Property(reference => reference.BranchId)
                        .HasConversion<NullableBranchIdConverter>()
                        .HasColumnName("CreatedByBranchId")
                        .HasMaxLength(64);

                    actor.Property(reference => reference.OfficeId)
                        .HasColumnName("CreatedByOfficeId")
                        .HasMaxLength(64);

                    actor.Property(reference => reference.HasAirlineOverride)
                        .HasColumnName("CreatedByHasAirlineOverride")
                        .IsRequired();
                });
            });
        }

        private static void ConfigureTravelers(EntityTypeBuilder<Order> builder)
        {
            builder.OwnsMany(order => order.Travelers, traveler =>
            {
                traveler.ToTable("Travelers", OrderingDbContext.OrderingSchema);
                traveler.WithOwner().HasForeignKey("OrderId");
                traveler.HasKey("OrderId", "Id");

                traveler.Property(entity => entity.Id)
                    .HasConversion<TravelerIdConverter>()
                    .HasColumnName("TravelerId")
                    .HasMaxLength(32);

                traveler.Property(entity => entity.Ptc)
                    .HasConversion<string>()
                    .HasMaxLength(8)
                    .IsRequired();

                traveler.Property(entity => entity.DateOfBirth)
                    .HasConversion<NullableLocalDateConverter>()
                    .HasColumnType("date");

                traveler.Property(entity => entity.AssociatedAdult)
                    .HasConversion<NullableTravelerIdConverter>()
                    .HasMaxLength(32);

                traveler.Property(entity => entity.CustomerRef)
                    .HasConversion<CustomerIdConverter>()
                    .HasMaxLength(128);

                traveler.Ignore(entity => entity.LastUpdateTime);
                traveler.Ignore(entity => entity.LastUpdatedBy);

                traveler.OwnsOne(entity => entity.Name, name =>
                {
                    name.Property(value => value.Given).HasColumnName("Given").HasMaxLength(128).IsRequired();
                    name.Property(value => value.Surname).HasColumnName("Surname").HasMaxLength(128).IsRequired();
                    name.Property(value => value.Title).HasColumnName("Title").HasMaxLength(32);
                    name.Property(value => value.GivenLocal).HasColumnName("GivenLocal").HasMaxLength(128);
                    name.Property(value => value.SurnameLocal).HasColumnName("SurnameLocal").HasMaxLength(128);
                });

                traveler.OwnsMany(entity => entity.Documents, document =>
                {
                    document.ToTable("TravelerDocuments", OrderingDbContext.OrderingSchema);
                    document.WithOwner().HasForeignKey("OrderId", "TravelerId");
                    document.Property<int>("Seq").ValueGeneratedOnAdd();
                    document.HasKey("OrderId", "TravelerId", "Seq");

                    document.Property(value => value.DocType)
                        .HasConversion<string>()
                        .HasMaxLength(32)
                        .IsRequired();

                    document.Property(value => value.Number).HasMaxLength(128).IsRequired();
                    document.Property(value => value.IssuingCountry).HasMaxLength(8);
                    document.Property(value => value.Nationality).HasMaxLength(8);

                    document.Property(value => value.Expiry)
                        .HasConversion<NullableLocalDateConverter>()
                        .HasColumnType("date");
                });
            });
        }

        private static void ConfigureJourneyElements(EntityTypeBuilder<Order> builder)
        {
            builder.OwnsMany(order => order.JourneyElements, element =>
            {
                element.ToTable("JourneyElements", OrderingDbContext.OrderingSchema);
                element.WithOwner().HasForeignKey("OrderId");
                element.HasKey("OrderId", "Id");

                element.Property(entity => entity.Id)
                    .HasConversion<JourneyElementIdConverter>()
                    .HasColumnName("JourneyElementId")
                    .HasMaxLength(32);

                element.Property(entity => entity.Cabin)
                    .HasConversion<CabinCodeConverter>()
                    .HasMaxLength(8)
                    .IsRequired();

                element.Property(entity => entity.Rbd)
                    .HasConversion<RbdCodeConverter>()
                    .HasMaxLength(8)
                    .IsRequired();

                element.Property(entity => entity.MarriedGroup)
                    .HasConversion<MarriedGroupIdConverter>()
                    .HasMaxLength(32);

                element.Property(entity => entity.SequenceInJourney).IsRequired();

                element.Ignore(entity => entity.LastUpdateTime);
                element.Ignore(entity => entity.LastUpdatedBy);

                element.OwnsOne(entity => entity.Flight, flight =>
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
        }

        private static void ConfigureSegments(EntityTypeBuilder<Order> builder)
        {
            builder.OwnsMany(order => order.Segments, segment =>
            {
                segment.ToTable("TravelerJourneySegments", OrderingDbContext.OrderingSchema);
                segment.WithOwner().HasForeignKey("OrderId");

                segment.Property(entity => entity.TravelerRef)
                    .HasConversion<TravelerIdConverter>()
                    .HasColumnName("TravelerId")
                    .HasMaxLength(32);

                segment.Property(entity => entity.JourneyRef)
                    .HasConversion<JourneyElementIdConverter>()
                    .HasColumnName("JourneyElementId")
                    .HasMaxLength(32);

                segment.HasKey("OrderId", "TravelerRef", "JourneyRef");

                segment.Property(entity => entity.SegmentStatus)
                    .HasConversion<string>().HasMaxLength(32).IsRequired();

                segment.Property(entity => entity.DeliveryStatus)
                    .HasConversion<string>().HasMaxLength(32).IsRequired();

                segment.Property(entity => entity.Hold)
                    .HasConversion<InventoryHoldRefConverter>()
                    .HasColumnName("InventoryHoldRef")
                    .HasMaxLength(128);

                segment.Ignore(entity => entity.IsUnderExternalControl);
                segment.Ignore(entity => entity.IsConsumed);

                segment.OwnsOne(entity => entity.Seat, seat =>
                {
                    seat.Property(value => value.SeatNumber).HasColumnName("SeatNumber").HasMaxLength(16);
                    seat.Property(value => value.AssignedAtUtc)
                        .HasConversion<InstantConverter>()
                        .HasColumnName("SeatAssignedAtUtc").HasColumnType("datetime2(7)");
                });

                segment.OwnsOne(entity => entity.DeliveryRef, delivery =>
                {
                    delivery.Property(value => value.DeliveryRecordNumber)
                        .HasColumnName("DeliveryRecordNumber").HasMaxLength(64);
                    delivery.Property(value => value.UnitNumber).HasColumnName("DeliveryUnitNumber");
                    delivery.Property(value => value.MirroredAtUtc)
                        .HasConversion<InstantConverter>()
                        .HasColumnName("MirroredAtUtc").HasColumnType("datetime2(7)");
                });
            });
        }

        private static void ConfigureItems(EntityTypeBuilder<Order> builder)
        {
            builder.OwnsMany(order => order.Items, item =>
            {
                item.ToTable("OrderItems", OrderingDbContext.OrderingSchema);
                item.WithOwner().HasForeignKey("OrderId");
                item.HasKey("OrderId", "Id");

                item.Property(entity => entity.Id)
                    .HasConversion<OrderItemIdConverter>()
                    .HasColumnName("OrderItemId")
                    .HasMaxLength(32);

                item.Property(entity => entity.Type).HasConversion<string>().HasMaxLength(32).IsRequired();
                item.Property(entity => entity.Status).HasConversion<string>().HasMaxLength(32).IsRequired();

                item.Property(entity => entity.TravelerRef)
                    .HasConversion<TravelerIdConverter>()
                    .HasColumnName("TravelerId")
                    .HasMaxLength(32)
                    .IsRequired();

                item.Property(entity => entity.ReplacesItem)
                    .HasConversion<NullableOrderItemIdConverter>()
                    .HasMaxLength(32);

                item.Property(entity => entity.CancellationReason)
                    .HasConversion<string>()
                    .HasMaxLength(32);

                item.Ignore(entity => entity.IsActive);
                item.Ignore(entity => entity.IsConfirmedOrBeyond);
                item.Ignore(entity => entity.IsDeliveredOrBeyond);
                item.Ignore(entity => entity.LastUpdateTime);
                item.Ignore(entity => entity.LastUpdatedBy);

                item.Property(entity => entity.JourneyRefs)
                    .HasConversion(
                        new JourneyElementIdListConverter(),
                        new JourneyElementIdListComparer())
                    .HasColumnName("JourneyRefs")
                    .HasMaxLength(512);

                item.OwnsOne(entity => entity.Product, product =>
                {
                    product.Property(value => value.ProductCode)
                        .HasColumnName("ProductCode").HasMaxLength(128).IsRequired();
                    product.Property(value => value.ProductType)
                        .HasConversion<string>().HasColumnName("ProductType").HasMaxLength(32).IsRequired();
                    product.Property(value => value.SourceSystemRef)
                        .HasColumnName("ProductSourceRef").HasMaxLength(128);
                });

                item.OwnsOne(entity => entity.SourceOffer, offer =>
                {
                    offer.Property(value => value.OfferId)
                        .HasConversion<OfferIdConverter>()
                        .HasColumnName("OfferId").HasMaxLength(128).IsRequired();
                    offer.Property(value => value.OfferItemId)
                        .HasColumnName("OfferItemId").HasMaxLength(128).IsRequired();
                    offer.Property(value => value.OfferExpiryUtc)
                        .HasConversion<InstantConverter>()
                        .HasColumnName("OfferExpiryUtc").HasColumnType("datetime2(7)").IsRequired();
                    offer.Property(value => value.OfferOwner)
                        .HasColumnName("OfferOwner").HasMaxLength(128).IsRequired();
                });

                item.OwnsOne(entity => entity.CreditAuthority, credit =>
                {
                    credit.Property(value => value.Value)
                        .HasColumnName("CreditAuthorityRef").HasMaxLength(128);
                    credit.Property(value => value.GrantedAtUtc)
                        .HasConversion<InstantConverter>()
                        .HasColumnName("CreditAuthorityGrantedAtUtc").HasColumnType("datetime2(7)");
                });

                item.OwnsOne(entity => entity.Partner, partner =>
                {
                    partner.Property(value => value.DeliveryOwner)
                        .HasConversion<CarrierCodeConverter>()
                        .HasColumnName("PartnerDeliveryOwner").HasMaxLength(8);
                    partner.Property(value => value.CommercialOwner)
                        .HasConversion<CarrierCodeConverter>()
                        .HasColumnName("PartnerCommercialOwner").HasMaxLength(8);
                    partner.Property(value => value.ServicingAuthority)
                        .HasConversion<string>()
                        .HasColumnName("PartnerServicingAuthority").HasMaxLength(32);
                    partner.Property(value => value.ExternalOrderReference)
                        .HasColumnName("PartnerExternalOrderReference").HasMaxLength(128);
                    partner.Property(value => value.ExternalItemReference)
                        .HasColumnName("PartnerExternalItemReference").HasMaxLength(128);
                    partner.Property(value => value.PartnerStatus)
                        .HasColumnName("PartnerStatus").HasMaxLength(64);
                    partner.Property(value => value.PartnerStatusMappedTo)
                        .HasConversion<string>()
                        .HasColumnName("PartnerStatusMappedTo").HasMaxLength(32);
                    partner.Property(value => value.ControlTransferState)
                        .HasConversion<string>()
                        .HasColumnName("PartnerControlTransferState").HasMaxLength(32);
                    partner.Property(value => value.RefundResponsibility)
                        .HasConversion<string>()
                        .HasColumnName("PartnerRefundResponsibility").HasMaxLength(32);
                    partner.Property(value => value.DisruptionResponsibility)
                        .HasConversion<string>()
                        .HasColumnName("PartnerDisruptionResponsibility").HasMaxLength(32);
                    partner.Property(value => value.SettlementBoundary)
                        .HasColumnName("PartnerSettlementBoundary").HasMaxLength(128);
                    partner.Property(value => value.ReconciliationReference)
                        .HasColumnName("PartnerReconciliationReference").HasMaxLength(128);
                });

                item.OwnsOne(entity => entity.Price, price =>
                {
                    price.Property(value => value.Currency)
                        .HasConversion<CurrencyCodeConverter>()
                        .HasColumnName("PriceCurrency").HasMaxLength(3).IsRequired();

                    price.Property(value => value.PricedAtUtc)
                        .HasConversion<InstantConverter>()
                        .HasColumnName("PricedAtUtc").HasColumnType("datetime2(7)").IsRequired();

                    price.Property(value => value.PricingEngineVersion)
                        .HasColumnName("PricingEngineVersion").HasMaxLength(64).IsRequired();

                    price.OwnsOne(value => value.Total, total =>
                    {
                        total.Property(money => money.Amount)
                            .HasColumnName("PriceTotal").HasColumnType("decimal(19,4)").IsRequired();
                        total.Property(money => money.Currency)
                            .HasConversion<CurrencyCodeConverter>()
                            .HasColumnName("PriceTotalCurrency").HasMaxLength(3).IsRequired();
                    });

                    price.OwnsOne(value => value.Fx, fx =>
                    {
                        fx.Property(snapshot => snapshot.From)
                            .HasConversion<CurrencyCodeConverter>()
                            .HasColumnName("FxFrom").HasMaxLength(3);
                        fx.Property(snapshot => snapshot.To)
                            .HasConversion<CurrencyCodeConverter>()
                            .HasColumnName("FxTo").HasMaxLength(3);
                        fx.Property(snapshot => snapshot.Rate)
                            .HasColumnName("FxRate").HasColumnType("decimal(19,8)");
                        fx.Property(snapshot => snapshot.RateSource)
                            .HasColumnName("FxSource").HasMaxLength(64);
                        fx.Property(snapshot => snapshot.CapturedAtUtc)
                            .HasConversion<InstantConverter>()
                            .HasColumnName("FxCapturedAtUtc").HasColumnType("datetime2(7)");
                    });

                    price.OwnsMany(value => value.Lines, line =>
                    {
                        line.ToTable("ChargeLines", OrderingDbContext.OrderingSchema);
                        line.WithOwner().HasForeignKey("OrderId", "OrderItemId");
                        line.Property<int>("Seq").ValueGeneratedOnAdd();
                        line.HasKey("OrderId", "OrderItemId", "Seq");

                        line.Property(charge => charge.ChargeType)
                            .HasConversion<string>().HasMaxLength(32).IsRequired();
                        line.Property(charge => charge.Code).HasMaxLength(64).IsRequired();
                        line.Property(charge => charge.Description).HasMaxLength(256);
                        line.Property(charge => charge.IsRefundable).IsRequired();
                        line.Property(charge => charge.TaxJurisdiction).HasMaxLength(32);

                        line.OwnsOne(charge => charge.Amount, money =>
                        {
                            money.Property(amount => amount.Amount)
                                .HasColumnName("Amount").HasColumnType("decimal(19,4)").IsRequired();
                            money.Property(amount => amount.Currency)
                                .HasConversion<CurrencyCodeConverter>()
                                .HasColumnName("Currency").HasMaxLength(3).IsRequired();
                        });
                    });

                    price.OwnsMany(value => value.ValueAllocations, allocation =>
                    {
                        allocation.ToTable("ValueAllocations", OrderingDbContext.OrderingSchema);
                        allocation.WithOwner().HasForeignKey("OrderId", "OrderItemId");
                        allocation.Property<int>("Seq").ValueGeneratedOnAdd();
                        allocation.HasKey("OrderId", "OrderItemId", "Seq");

                        allocation.Property(value => value.TravelerRef)
                            .HasConversion<TravelerIdConverter>()
                            .HasColumnName("TravelerId").HasMaxLength(32).IsRequired();

                        allocation.Property(value => value.JourneyRef)
                            .HasConversion<JourneyElementIdConverter>()
                            .HasColumnName("JourneyElementId").HasMaxLength(32).IsRequired();

                        allocation.Property(value => value.AllocationVersion).HasMaxLength(64).IsRequired();

                        allocation.Property(value => value.ResidualPolicy)
                            .HasConversion<string>().HasMaxLength(32).IsRequired();

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
            });
        }


        private static void ConfigurePayments(EntityTypeBuilder<Order> builder)
        {
            builder.OwnsMany(order => order.Payments, payment =>
            {
                payment.ToTable("PaymentRecords", OrderingDbContext.OrderingSchema);
                payment.WithOwner().HasForeignKey("OrderId");
                payment.HasKey("OrderId", "Id");

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
                    .HasConversion<InstantConverter>()
                    .HasColumnType("datetime2(7)").IsRequired();

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
                        .HasConversion<CurrencyCodeConverter>()
                        .HasColumnName("FxFrom").HasMaxLength(3);
                    fx.Property(value => value.To)
                        .HasConversion<CurrencyCodeConverter>()
                        .HasColumnName("FxTo").HasMaxLength(3);
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
                    allocation.ToTable("PaymentAllocations", OrderingDbContext.OrderingSchema);
                    allocation.WithOwner().HasForeignKey("OrderId", "PaymentRecordId");

                    allocation.Property(value => value.ItemRef)
                        .HasConversion<OrderItemIdConverter>()
                        .HasColumnName("OrderItemId").HasMaxLength(32);

                    allocation.HasKey("OrderId", "PaymentRecordId", "ItemRef");

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
        }

        private static void ConfigureTimeLimits(EntityTypeBuilder<Order> builder)
        {
            builder.OwnsMany(order => order.TimeLimits, timeLimit =>
            {
                timeLimit.ToTable("TimeLimits", OrderingDbContext.OrderingSchema);
                timeLimit.WithOwner().HasForeignKey("OrderId");
                timeLimit.HasKey("OrderId", "Id");

                timeLimit.Property(entity => entity.Id)
                    .HasConversion<TimeLimitIdConverter>()
                    .HasColumnName("TimeLimitId").HasMaxLength(32);

                timeLimit.Property(entity => entity.Type)
                    .HasConversion<string>().HasMaxLength(32).IsRequired();

                timeLimit.Property(entity => entity.Status)
                    .HasConversion<string>().HasMaxLength(32).IsRequired();

                timeLimit.Property(entity => entity.DueAtUtc)
                    .HasConversion<InstantConverter>()
                    .HasColumnType("datetime2(7)").IsRequired();

                timeLimit.Property(entity => entity.ExtensionCount).IsRequired();

                timeLimit.Ignore(entity => entity.IsActive);
                timeLimit.Ignore(entity => entity.LastUpdateTime);
                timeLimit.Ignore(entity => entity.LastUpdatedBy);

                timeLimit.HasIndex(entity => new { entity.Status, entity.DueAtUtc })
                    .HasFilter("[Status] = 'Active'");

                timeLimit.Property(entity => entity.AppliesTo)
                    .HasConversion(
                        new OrderItemIdListConverter(),
                        new OrderItemIdListComparer())
                    .HasColumnName("AppliesTo")
                    .HasMaxLength(512);
            });
        }

        private static void ConfigureContacts(EntityTypeBuilder<Order> builder)
        {
            builder.OwnsMany(order => order.Contacts, contact =>
            {
                contact.ToTable("ContactPoints", OrderingDbContext.OrderingSchema);
                contact.WithOwner().HasForeignKey("OrderId");
                contact.Property<int>("Seq").ValueGeneratedOnAdd();
                contact.HasKey("OrderId", "Seq");

                contact.Property(value => value.Type)
                    .HasConversion<string>().HasMaxLength(32).IsRequired();
                contact.Property(value => value.Value).HasMaxLength(256).IsRequired();
                contact.Property(value => value.IsPrimary).IsRequired();
            });
        }

        private static void ConfigureHistory(EntityTypeBuilder<Order> builder)
        {
            builder.OwnsMany(order => order.History, entry =>
            {
                entry.ToTable("OrderHistory", OrderingDbContext.AuditSchema);
                entry.WithOwner().HasForeignKey("OrderId");
                entry.HasKey("OrderId", "SequenceNo");

                entry.Property(value => value.SequenceNo).ValueGeneratedNever();

                entry.Property(value => value.OccurredAtUtc)
                    .HasConversion<InstantConverter>()
                    .HasColumnType("datetime2(7)").IsRequired();

                entry.Property(value => value.CommandName).HasMaxLength(128).IsRequired();

                entry.Property(value => value.Channel)
                    .HasConversion<string>().HasColumnName("ChannelCode").HasMaxLength(32).IsRequired();

                entry.Property(value => value.CorrelationId).HasMaxLength(128).IsRequired();
                entry.Property(value => value.CausationId).HasMaxLength(128);
                entry.Property(value => value.ResultingAggregateVersion).IsRequired();

                entry.OwnsOne(value => value.Actor, actor =>
                {
                    actor.Property(reference => reference.ActorType)
                        .HasConversion<string>().HasColumnName("ActorType").HasMaxLength(32).IsRequired();
                    actor.Property(reference => reference.UserId)
                        .HasColumnName("ActorUserId").HasMaxLength(128).IsRequired();
                    actor.Property(reference => reference.SellerId)
                        .HasConversion<NullableSellerIdConverter>()
                        .HasColumnName("ActorSellerId").HasMaxLength(64);
                    actor.Property(reference => reference.BranchId)
                        .HasConversion<NullableBranchIdConverter>()
                        .HasColumnName("ActorBranchId").HasMaxLength(64);

                    actor.Property(reference => reference.OfficeId)
                        .HasColumnName("ActorOfficeId").HasMaxLength(64);
                    actor.Property(reference => reference.HasAirlineOverride)
                        .HasColumnName("ActorHasAirlineOverride").IsRequired();
                });
            });
        }
    }
}
