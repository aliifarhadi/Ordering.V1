using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AeroTech.Ordering.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AllContextsDomain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Delivery");

            migrationBuilder.AddColumn<string>(
                name: "BranchId",
                schema: "Ordering",
                table: "Orders",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByBranchId",
                schema: "Ordering",
                table: "Orders",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "GroupBookingId",
                schema: "Ordering",
                table: "Orders",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ActorBranchId",
                schema: "Audit",
                table: "OrderHistory",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Detail",
                schema: "Ops",
                table: "OperationSteps",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "OrderId",
                schema: "Ops",
                table: "Operations",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "GroupBookingId",
                schema: "Ops",
                table: "Operations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DeliveryRecords",
                schema: "Delivery",
                columns: table => new
                {
                    DeliveryRecordNumber = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TravelerId = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Given = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Surname = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    GivenLocal = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    SurnameLocal = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Ptc = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: true),
                    DocumentType = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    DocumentNumber = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    DocumentIssuingCountry = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: true),
                    DocumentExpiry = table.Column<DateOnly>(type: "date", nullable: true),
                    DocumentNationality = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: true),
                    BranchId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    IssuedAtUtc = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    VoidWindowHours = table.Column<int>(type: "int", nullable: false),
                    AggregateVersion = table.Column<long>(type: "bigint", nullable: false),
                    LastUpdateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryRecords", x => x.DeliveryRecordNumber);
                });

            migrationBuilder.CreateTable(
                name: "DocumentNumberRanges",
                schema: "Delivery",
                columns: table => new
                {
                    RangeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    DocumentKind = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Prefix = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    RangeStart = table.Column<long>(type: "bigint", nullable: false),
                    RangeEnd = table.Column<long>(type: "bigint", nullable: false),
                    NextAvailable = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    RegisteredWithAuthorityAtUtc = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    AuthorityReference = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    LastUpdateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentNumberRanges", x => x.RangeId);
                });

            migrationBuilder.CreateTable(
                name: "GroupBookings",
                schema: "Ordering",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Reference = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    SellerId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    BranchId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    ChannelCode = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    OfficeId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    PointOfSale = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    SaleCurrency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    SoldAtUtc = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    CreatedByActorType = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    CreatedBySellerId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    CreatedByBranchId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    CreatedByOfficeId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    CreatedByHasAirlineOverride = table.Column<bool>(type: "bit", nullable: false),
                    AggregateVersion = table.Column<long>(type: "bigint", nullable: false),
                    IsSuspended = table.Column<bool>(type: "bit", nullable: false),
                    ClosedAtUtc = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    BlocksConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    SpawnedOrders = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastUpdateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupBookings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaxDocuments",
                schema: "Delivery",
                columns: table => new
                {
                    DocumentNumber = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Kind = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeliveryRecordNumber = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    ReversesDocumentNumber = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    BranchId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    SpecVersion = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    BuyerTaxIdentity = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    PassengerIdentity = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    IssuedAtUtc = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    TotalTax = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    TotalTaxCurrency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    SubmissionStatus = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    AggregateVersion = table.Column<long>(type: "bigint", nullable: false),
                    LastUpdateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxDocuments", x => x.DocumentNumber);
                });

            migrationBuilder.CreateTable(
                name: "SegmentDeliveries",
                schema: "Delivery",
                columns: table => new
                {
                    UnitNumber = table.Column<int>(type: "int", nullable: false),
                    DeliveryRecordNumber = table.Column<string>(type: "nvarchar(64)", nullable: false),
                    OrderItemId = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    JourneyElementId = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
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
                    Status = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    ControlHolder = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    ControlAcquiredAtUtc = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    ControlLeaseExpiresAtUtc = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    UnitValue = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SegmentDeliveries", x => new { x.DeliveryRecordNumber, x.UnitNumber });
                    table.ForeignKey(
                        name: "FK_SegmentDeliveries_DeliveryRecords_DeliveryRecordNumber",
                        column: x => x.DeliveryRecordNumber,
                        principalSchema: "Delivery",
                        principalTable: "DeliveryRecords",
                        principalColumn: "DeliveryRecordNumber",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceDeliveries",
                schema: "Delivery",
                columns: table => new
                {
                    UnitNumber = table.Column<int>(type: "int", nullable: false),
                    DeliveryRecordNumber = table.Column<string>(type: "nvarchar(64)", nullable: false),
                    OrderItemId = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    ServiceCode = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    UnitValue = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    AssociatedSegmentUnit = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceDeliveries", x => new { x.DeliveryRecordNumber, x.UnitNumber });
                    table.ForeignKey(
                        name: "FK_ServiceDeliveries_DeliveryRecords_DeliveryRecordNumber",
                        column: x => x.DeliveryRecordNumber,
                        principalSchema: "Delivery",
                        principalTable: "DeliveryRecords",
                        principalColumn: "DeliveryRecordNumber",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroupNameSlots",
                schema: "Ordering",
                columns: table => new
                {
                    SlotId = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    GroupBookingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Given = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Surname = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    GivenLocal = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    SurnameLocal = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Ptc = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: true),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupNameSlots", x => new { x.GroupBookingId, x.SlotId });
                    table.ForeignKey(
                        name: "FK_GroupNameSlots_GroupBookings_GroupBookingId",
                        column: x => x.GroupBookingId,
                        principalSchema: "Ordering",
                        principalTable: "GroupBookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroupPaymentRecords",
                schema: "Ordering",
                columns: table => new
                {
                    PaymentRecordId = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    GroupBookingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    table.PrimaryKey("PK_GroupPaymentRecords", x => new { x.GroupBookingId, x.PaymentRecordId });
                    table.ForeignKey(
                        name: "FK_GroupPaymentRecords_GroupBookings_GroupBookingId",
                        column: x => x.GroupBookingId,
                        principalSchema: "Ordering",
                        principalTable: "GroupBookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroupSeatBlocks",
                schema: "Ordering",
                columns: table => new
                {
                    BlockId = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    GroupBookingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    SeatsHeld = table.Column<int>(type: "int", nullable: false),
                    SeatsAllocated = table.Column<int>(type: "int", nullable: false),
                    SeatsReleased = table.Column<int>(type: "int", nullable: false),
                    InventoryBlockRef = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupSeatBlocks", x => new { x.GroupBookingId, x.BlockId });
                    table.ForeignKey(
                        name: "FK_GroupSeatBlocks_GroupBookings_GroupBookingId",
                        column: x => x.GroupBookingId,
                        principalSchema: "Ordering",
                        principalTable: "GroupBookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroupTimeLimits",
                schema: "Ordering",
                columns: table => new
                {
                    TimeLimitId = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    GroupBookingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    DueAtUtc = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    AppliesTo = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    ExtensionCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupTimeLimits", x => new { x.GroupBookingId, x.TimeLimitId });
                    table.ForeignKey(
                        name: "FK_GroupTimeLimits_GroupBookings_GroupBookingId",
                        column: x => x.GroupBookingId,
                        principalSchema: "Ordering",
                        principalTable: "GroupBookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaxDocumentLines",
                schema: "Delivery",
                columns: table => new
                {
                    Seq = table.Column<int>(type: "int", nullable: false),
                    DocumentNumber = table.Column<string>(type: "nvarchar(64)", nullable: false),
                    LineType = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    NetAmount = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    TaxAmount = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    TaxCurrency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    TaxRate = table.Column<decimal>(type: "decimal(9,6)", precision: 19, scale: 4, nullable: true),
                    TaxJurisdiction = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxDocumentLines", x => new { x.DocumentNumber, x.Seq });
                    table.ForeignKey(
                        name: "FK_TaxDocumentLines_TaxDocuments_DocumentNumber",
                        column: x => x.DocumentNumber,
                        principalSchema: "Delivery",
                        principalTable: "TaxDocuments",
                        principalColumn: "DocumentNumber",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaxDocumentSubmissionAttempts",
                schema: "Delivery",
                columns: table => new
                {
                    Seq = table.Column<int>(type: "int", nullable: false),
                    DocumentNumber = table.Column<string>(type: "nvarchar(64)", nullable: false),
                    AttemptedAtUtc = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    Outcome = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    ResponseReference = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxDocumentSubmissionAttempts", x => new { x.DocumentNumber, x.Seq });
                    table.ForeignKey(
                        name: "FK_TaxDocumentSubmissionAttempts_TaxDocuments_DocumentNumber",
                        column: x => x.DocumentNumber,
                        principalSchema: "Delivery",
                        principalTable: "TaxDocuments",
                        principalColumn: "DocumentNumber",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SegmentDeliveryStatusHistory",
                schema: "Delivery",
                columns: table => new
                {
                    Seq = table.Column<int>(type: "int", nullable: false),
                    DeliveryRecordNumber = table.Column<string>(type: "nvarchar(64)", nullable: false),
                    UnitNumber = table.Column<int>(type: "int", nullable: false),
                    FromStatus = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    ToStatus = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    OccurredAtUtc = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    Source = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SegmentDeliveryStatusHistory", x => new { x.DeliveryRecordNumber, x.UnitNumber, x.Seq });
                    table.ForeignKey(
                        name: "FK_SegmentDeliveryStatusHistory_SegmentDeliveries_DeliveryRecordNumber_UnitNumber",
                        columns: x => new { x.DeliveryRecordNumber, x.UnitNumber },
                        principalSchema: "Delivery",
                        principalTable: "SegmentDeliveries",
                        principalColumns: new[] { "DeliveryRecordNumber", "UnitNumber" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroupPaymentAllocations",
                schema: "Ordering",
                columns: table => new
                {
                    OrderItemId = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    GroupBookingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentRecordId = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupPaymentAllocations", x => new { x.GroupBookingId, x.PaymentRecordId, x.OrderItemId });
                    table.ForeignKey(
                        name: "FK_GroupPaymentAllocations_GroupPaymentRecords_GroupBookingId_PaymentRecordId",
                        columns: x => new { x.GroupBookingId, x.PaymentRecordId },
                        principalSchema: "Ordering",
                        principalTable: "GroupPaymentRecords",
                        principalColumns: new[] { "GroupBookingId", "PaymentRecordId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryRecords_OrderId",
                schema: "Delivery",
                table: "DeliveryRecords",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentNumberRanges_BranchId_DocumentKind_Status",
                schema: "Delivery",
                table: "DocumentNumberRanges",
                columns: new[] { "BranchId", "DocumentKind", "Status" },
                unique: true,
                filter: "[Status] = 'Active'");

            migrationBuilder.CreateIndex(
                name: "IX_GroupBookings_Reference",
                schema: "Ordering",
                table: "GroupBookings",
                column: "Reference",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GroupSeatBlocks_MarketingCarrier_FlightNumber_DepartureLocalDate",
                schema: "Ordering",
                table: "GroupSeatBlocks",
                columns: new[] { "MarketingCarrier", "FlightNumber", "DepartureLocalDate" });

            migrationBuilder.CreateIndex(
                name: "IX_GroupTimeLimits_Status_DueAtUtc",
                schema: "Ordering",
                table: "GroupTimeLimits",
                columns: new[] { "Status", "DueAtUtc" },
                filter: "[Status] = 'Active'");

            migrationBuilder.CreateIndex(
                name: "IX_SegmentDeliveries_MarketingCarrier_FlightNumber_DepartureLocalDate",
                schema: "Delivery",
                table: "SegmentDeliveries",
                columns: new[] { "MarketingCarrier", "FlightNumber", "DepartureLocalDate" });

            migrationBuilder.CreateIndex(
                name: "IX_TaxDocuments_OrderId",
                schema: "Delivery",
                table: "TaxDocuments",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_TaxDocuments_SubmissionStatus_IssuedAtUtc",
                schema: "Delivery",
                table: "TaxDocuments",
                columns: new[] { "SubmissionStatus", "IssuedAtUtc" },
                filter: "[SubmissionStatus] = 'PendingSubmission'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentNumberRanges",
                schema: "Delivery");

            migrationBuilder.DropTable(
                name: "GroupNameSlots",
                schema: "Ordering");

            migrationBuilder.DropTable(
                name: "GroupPaymentAllocations",
                schema: "Ordering");

            migrationBuilder.DropTable(
                name: "GroupSeatBlocks",
                schema: "Ordering");

            migrationBuilder.DropTable(
                name: "GroupTimeLimits",
                schema: "Ordering");

            migrationBuilder.DropTable(
                name: "SegmentDeliveryStatusHistory",
                schema: "Delivery");

            migrationBuilder.DropTable(
                name: "ServiceDeliveries",
                schema: "Delivery");

            migrationBuilder.DropTable(
                name: "TaxDocumentLines",
                schema: "Delivery");

            migrationBuilder.DropTable(
                name: "TaxDocumentSubmissionAttempts",
                schema: "Delivery");

            migrationBuilder.DropTable(
                name: "GroupPaymentRecords",
                schema: "Ordering");

            migrationBuilder.DropTable(
                name: "SegmentDeliveries",
                schema: "Delivery");

            migrationBuilder.DropTable(
                name: "TaxDocuments",
                schema: "Delivery");

            migrationBuilder.DropTable(
                name: "GroupBookings",
                schema: "Ordering");

            migrationBuilder.DropTable(
                name: "DeliveryRecords",
                schema: "Delivery");

            migrationBuilder.DropColumn(
                name: "BranchId",
                schema: "Ordering",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CreatedByBranchId",
                schema: "Ordering",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "GroupBookingId",
                schema: "Ordering",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ActorBranchId",
                schema: "Audit",
                table: "OrderHistory");

            migrationBuilder.DropColumn(
                name: "Detail",
                schema: "Ops",
                table: "OperationSteps");

            migrationBuilder.DropColumn(
                name: "GroupBookingId",
                schema: "Ops",
                table: "Operations");

            migrationBuilder.AlterColumn<Guid>(
                name: "OrderId",
                schema: "Ops",
                table: "Operations",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }
    }
}
