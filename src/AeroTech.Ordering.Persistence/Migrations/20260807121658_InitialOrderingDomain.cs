using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AeroTech.Ordering.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialOrderingDomain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Ordering");

            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.EnsureSchema(
                name: "Ops");

            migrationBuilder.EnsureSchema(
                name: "Audit");

            migrationBuilder.CreateTable(
                name: "InboxMessages",
                schema: "dbo",
                columns: table => new
                {
                    MessageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Consumer = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    MessageType = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ReceivedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InboxMessages", x => new { x.MessageId, x.Consumer });
                });

            migrationBuilder.CreateTable(
                name: "Operations",
                schema: "Ops",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    CurrentStep = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ExpectedExternalMessage = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CorrelationId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    CausationId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    AttemptCount = table.Column<int>(type: "int", nullable: false),
                    NextRetryAtUtc = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    TimeoutAtUtc = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    CompensationPolicy = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Result = table.Column<string>(type: "nvarchar(max)", maxLength: 256, nullable: true),
                    FailureReason = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    IdempotencyKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ScopedItems = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    LastUpdateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Operations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                schema: "Ordering",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Reference = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    SellerId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    ChannelCode = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    OfficeId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    PointOfSale = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    SaleCurrency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    SoldAtUtc = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    CreatedByActorType = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    CreatedBySellerId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    CreatedByOfficeId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    CreatedByHasAirlineOverride = table.Column<bool>(type: "bit", nullable: false),
                    AggregateVersion = table.Column<long>(type: "bigint", nullable: false),
                    ClosedAtUtc = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    IsSuspended = table.Column<bool>(type: "bit", nullable: false),
                    IsUnderLegalHold = table.Column<bool>(type: "bit", nullable: false),
                    RequiresReview = table.Column<bool>(type: "bit", nullable: false),
                    LastUpdateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OutboxMessages",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MessageType = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Payload = table.Column<string>(type: "nvarchar(max)", maxLength: 256, nullable: false),
                    OccurredOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ProcessedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OperationSteps",
                schema: "Ops",
                columns: table => new
                {
                    OperationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Seq = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    EnteredAtUtc = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    CompletedAtUtc = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    Outcome = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperationSteps", x => new { x.OperationId, x.Seq });
                    table.ForeignKey(
                        name: "FK_OperationSteps_Operations_OperationId",
                        column: x => x.OperationId,
                        principalSchema: "Ops",
                        principalTable: "Operations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContactPoints",
                schema: "Ordering",
                columns: table => new
                {
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Seq = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactPoints", x => new { x.OrderId, x.Seq });
                    table.ForeignKey(
                        name: "FK_ContactPoints_Orders_OrderId",
                        column: x => x.OrderId,
                        principalSchema: "Ordering",
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JourneyElements",
                schema: "Ordering",
                columns: table => new
                {
                    JourneyElementId = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MarketingCarrier = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    OperatingCarrier = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    FlightNumber = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    Origin = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    Destination = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    DepartureUtc = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    DepartureLocalDate = table.Column<DateOnly>(type: "date", nullable: false),
                    DepartureLocalTime = table.Column<TimeOnly>(type: "time(0)", nullable: false),
                    OriginTimeZoneId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    ArrivalUtc = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    ArrivalLocalDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ArrivalLocalTime = table.Column<TimeOnly>(type: "time(0)", nullable: false),
                    DestinationTimeZoneId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Cabin = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    Rbd = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    MarriedGroup = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    SequenceInJourney = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JourneyElements", x => new { x.OrderId, x.JourneyElementId });
                    table.ForeignKey(
                        name: "FK_JourneyElements_Orders_OrderId",
                        column: x => x.OrderId,
                        principalSchema: "Ordering",
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderHistory",
                schema: "Audit",
                columns: table => new
                {
                    SequenceNo = table.Column<long>(type: "bigint", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OccurredAtUtc = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    CommandName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ActorType = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    ActorUserId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ActorSellerId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    ActorOfficeId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    ActorHasAirlineOverride = table.Column<bool>(type: "bit", nullable: false),
                    ChannelCode = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    CorrelationId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    CausationId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ResultingAggregateVersion = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderHistory", x => new { x.OrderId, x.SequenceNo });
                    table.ForeignKey(
                        name: "FK_OrderHistory_Orders_OrderId",
                        column: x => x.OrderId,
                        principalSchema: "Ordering",
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                schema: "Ordering",
                columns: table => new
                {
                    OrderItemId = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    TravelerId = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    JourneyRefs = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    ProductCode = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProductType = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    ProductSourceRef = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    PriceTotal = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    PriceTotalCurrency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    PriceCurrency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    FxFrom = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    FxTo = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    FxRate = table.Column<decimal>(type: "decimal(19,8)", precision: 19, scale: 4, nullable: true),
                    FxSource = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    FxCapturedAtUtc = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    PricedAtUtc = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    PricingEngineVersion = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    OfferId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    OfferItemId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    OfferExpiryUtc = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    OfferOwner = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ReplacesItem = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    PartnerDeliveryOwner = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: true),
                    PartnerCommercialOwner = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: true),
                    PartnerServicingAuthority = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    PartnerExternalOrderReference = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    PartnerExternalItemReference = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    PartnerStatus = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    PartnerStatusMappedTo = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    PartnerControlTransferState = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    PartnerRefundResponsibility = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    PartnerDisruptionResponsibility = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    PartnerSettlementBoundary = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    PartnerReconciliationReference = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CancellationReason = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    CreditAuthorityRef = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreditAuthorityGrantedAtUtc = table.Column<DateTime>(type: "datetime2(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => new { x.OrderId, x.OrderItemId });
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalSchema: "Ordering",
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaymentRecords",
                schema: "Ordering",
                columns: table => new
                {
                    PaymentRecordId = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FormOfPayment = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    PaymentRequestRef = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    FxFrom = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    FxTo = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    FxRate = table.Column<decimal>(type: "decimal(19,8)", precision: 19, scale: 4, nullable: true),
                    FxSource = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    FxCapturedAtUtc = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    PayerRef = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    RecordedAtUtc = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    IsUnderDispute = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentRecords", x => new { x.OrderId, x.PaymentRecordId });
                    table.ForeignKey(
                        name: "FK_PaymentRecords_Orders_OrderId",
                        column: x => x.OrderId,
                        principalSchema: "Ordering",
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TimeLimits",
                schema: "Ordering",
                columns: table => new
                {
                    TimeLimitId = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    DueAtUtc = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    AppliesTo = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    ExtensionCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeLimits", x => new { x.OrderId, x.TimeLimitId });
                    table.ForeignKey(
                        name: "FK_TimeLimits_Orders_OrderId",
                        column: x => x.OrderId,
                        principalSchema: "Ordering",
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TravelerJourneySegments",
                schema: "Ordering",
                columns: table => new
                {
                    TravelerId = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    JourneyElementId = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SegmentStatus = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    DeliveryStatus = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    InventoryHoldRef = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    SeatNumber = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    SeatAssignedAtUtc = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    DeliveryRecordNumber = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    DeliveryUnitNumber = table.Column<int>(type: "int", nullable: true),
                    MirroredAtUtc = table.Column<DateTime>(type: "datetime2(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TravelerJourneySegments", x => new { x.OrderId, x.TravelerId, x.JourneyElementId });
                    table.ForeignKey(
                        name: "FK_TravelerJourneySegments_Orders_OrderId",
                        column: x => x.OrderId,
                        principalSchema: "Ordering",
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Travelers",
                schema: "Ordering",
                columns: table => new
                {
                    TravelerId = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Given = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Surname = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    GivenLocal = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    SurnameLocal = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Ptc = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: true),
                    AssociatedAdult = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    CustomerRef = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Travelers", x => new { x.OrderId, x.TravelerId });
                    table.ForeignKey(
                        name: "FK_Travelers_Orders_OrderId",
                        column: x => x.OrderId,
                        principalSchema: "Ordering",
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChargeLines",
                schema: "Ordering",
                columns: table => new
                {
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderItemId = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    Seq = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChargeType = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    IsRefundable = table.Column<bool>(type: "bit", nullable: false),
                    TaxJurisdiction = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChargeLines", x => new { x.OrderId, x.OrderItemId, x.Seq });
                    table.ForeignKey(
                        name: "FK_ChargeLines_OrderItems_OrderId_OrderItemId",
                        columns: x => new { x.OrderId, x.OrderItemId },
                        principalSchema: "Ordering",
                        principalTable: "OrderItems",
                        principalColumns: new[] { "OrderId", "OrderItemId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ValueAllocations",
                schema: "Ordering",
                columns: table => new
                {
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderItemId = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    Seq = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TravelerId = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    JourneyElementId = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    AllocationVersion = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    ResidualPolicy = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ValueAllocations", x => new { x.OrderId, x.OrderItemId, x.Seq });
                    table.ForeignKey(
                        name: "FK_ValueAllocations_OrderItems_OrderId_OrderItemId",
                        columns: x => new { x.OrderId, x.OrderItemId },
                        principalSchema: "Ordering",
                        principalTable: "OrderItems",
                        principalColumns: new[] { "OrderId", "OrderItemId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaymentAllocations",
                schema: "Ordering",
                columns: table => new
                {
                    OrderItemId = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentRecordId = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentAllocations", x => new { x.OrderId, x.PaymentRecordId, x.OrderItemId });
                    table.ForeignKey(
                        name: "FK_PaymentAllocations_PaymentRecords_OrderId_PaymentRecordId",
                        columns: x => new { x.OrderId, x.PaymentRecordId },
                        principalSchema: "Ordering",
                        principalTable: "PaymentRecords",
                        principalColumns: new[] { "OrderId", "PaymentRecordId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TravelerDocuments",
                schema: "Ordering",
                columns: table => new
                {
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TravelerId = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    Seq = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocType = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Number = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    IssuingCountry = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: true),
                    Expiry = table.Column<DateOnly>(type: "date", nullable: true),
                    Nationality = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TravelerDocuments", x => new { x.OrderId, x.TravelerId, x.Seq });
                    table.ForeignKey(
                        name: "FK_TravelerDocuments_Travelers_OrderId_TravelerId",
                        columns: x => new { x.OrderId, x.TravelerId },
                        principalSchema: "Ordering",
                        principalTable: "Travelers",
                        principalColumns: new[] { "OrderId", "TravelerId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InboxMessages_ReceivedOn",
                schema: "dbo",
                table: "InboxMessages",
                column: "ReceivedOn");

            migrationBuilder.CreateIndex(
                name: "IX_JourneyElements_MarketingCarrier_FlightNumber_DepartureLocalDate",
                schema: "Ordering",
                table: "JourneyElements",
                columns: new[] { "MarketingCarrier", "FlightNumber", "DepartureLocalDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Operations_IdempotencyKey",
                schema: "Ops",
                table: "Operations",
                column: "IdempotencyKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Operations_OrderId_Type_Status",
                schema: "Ops",
                table: "Operations",
                columns: new[] { "OrderId", "Type", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Operations_Status_NextRetryAtUtc",
                schema: "Ops",
                table: "Operations",
                columns: new[] { "Status", "NextRetryAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_Operations_Status_TimeoutAtUtc",
                schema: "Ops",
                table: "Operations",
                columns: new[] { "Status", "TimeoutAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_Reference",
                schema: "Ordering",
                table: "Orders",
                column: "Reference",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_ProcessedOn",
                schema: "dbo",
                table: "OutboxMessages",
                column: "ProcessedOn");

            migrationBuilder.CreateIndex(
                name: "IX_TimeLimits_Status_DueAtUtc",
                schema: "Ordering",
                table: "TimeLimits",
                columns: new[] { "Status", "DueAtUtc" },
                filter: "[Status] = 'Active'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChargeLines",
                schema: "Ordering");

            migrationBuilder.DropTable(
                name: "ContactPoints",
                schema: "Ordering");

            migrationBuilder.DropTable(
                name: "InboxMessages",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "JourneyElements",
                schema: "Ordering");

            migrationBuilder.DropTable(
                name: "OperationSteps",
                schema: "Ops");

            migrationBuilder.DropTable(
                name: "OrderHistory",
                schema: "Audit");

            migrationBuilder.DropTable(
                name: "OutboxMessages",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PaymentAllocations",
                schema: "Ordering");

            migrationBuilder.DropTable(
                name: "TimeLimits",
                schema: "Ordering");

            migrationBuilder.DropTable(
                name: "TravelerDocuments",
                schema: "Ordering");

            migrationBuilder.DropTable(
                name: "TravelerJourneySegments",
                schema: "Ordering");

            migrationBuilder.DropTable(
                name: "ValueAllocations",
                schema: "Ordering");

            migrationBuilder.DropTable(
                name: "Operations",
                schema: "Ops");

            migrationBuilder.DropTable(
                name: "PaymentRecords",
                schema: "Ordering");

            migrationBuilder.DropTable(
                name: "Travelers",
                schema: "Ordering");

            migrationBuilder.DropTable(
                name: "OrderItems",
                schema: "Ordering");

            migrationBuilder.DropTable(
                name: "Orders",
                schema: "Ordering");
        }
    }
}
