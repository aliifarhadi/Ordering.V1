/*  01-order-created.sql
    Order K7QM24 in Draft -> Pending: travellers, itinerary, segments, priced items,
    charge lines, value allocations, contacts, payment time limit.

    Scenario  THR -> MHD (2 Sep) / MHD -> THR (9 Sep), two adults.
      OI000001 flight  Ali   both legs      multi-segment, needs ValueAllocation (I-15)
      OI000002 flight  Sara  both legs      multi-segment, needs ValueAllocation (I-15)
      OI000003 bag     Ali   outbound       associated ancillary   (IATA EMD-A equivalent)
      OI000004 lounge  Sara  outbound       standalone ancillary   (IATA EMD-S equivalent)

    Depends on: 00-reset.sql
*/
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;
BEGIN TRANSACTION;

DECLARE @OrderId uniqueidentifier = '0195f3a0-1111-7000-8000-000000000001';
DECLARE @Now datetime2(7) = '2026-08-07T09:00:00';

INSERT INTO Ordering.Orders
    (Id, Reference, GroupBookingId, SellerId, BranchId, ChannelCode, OfficeId, PointOfSale,
     SaleCurrency, SoldAtUtc, CreatedByActorType, CreatedByUserId, CreatedBySellerId,
     CreatedByBranchId, CreatedByOfficeId, CreatedByHasAirlineOverride,
     AggregateVersion, ClosedAtUtc, IsSuspended, IsUnderLegalHold, RequiresReview,
     LastUpdateTime, LastUpdatedBy)
VALUES
    (@OrderId, 'K7QM24', NULL, 'SELLER-DOTAIR-DIRECT', 'BRANCH-THR-01', 'DirectIbe', 'OFFICE-THR-IBE', 'IR',
     'IRR', @Now, 'Agent', 'user-4471', 'SELLER-DOTAIR-DIRECT',
     'BRANCH-THR-01', 'OFFICE-THR-IBE', 0,
     12, NULL, 0, 0, 0,
     '2026-08-07T09:05:00+00:00', 4471);

INSERT INTO Ordering.Travelers
    (OrderId, TravelerId, Given, Surname, Title, GivenLocal, SurnameLocal, Ptc, DateOfBirth, AssociatedAdult, CustomerRef)
VALUES
    (@OrderId, 'TR000001', 'ALI', 'REZAEI', 'MR', N'علی', N'رضایی', 'ADT', '1988-04-12', NULL, 'CUST-88231'),
    (@OrderId, 'TR000002', 'SARA', 'MORADI', 'MS', N'سارا', N'مرادی', 'ADT', '1991-11-03', NULL, NULL);

SET IDENTITY_INSERT Ordering.TravelerDocuments ON;
INSERT INTO Ordering.TravelerDocuments
    (OrderId, TravelerId, Seq, DocType, Number, IssuingCountry, Expiry, Nationality)
VALUES
    (@OrderId, 'TR000001', 1, 'NationalId', '0064328871', 'IR', NULL, 'IR'),
    (@OrderId, 'TR000002', 1, 'NationalId', '0079914452', 'IR', NULL, 'IR');
SET IDENTITY_INSERT Ordering.TravelerDocuments OFF;

SET IDENTITY_INSERT Ordering.ContactPoints ON;
INSERT INTO Ordering.ContactPoints (OrderId, Seq, Type, Value, IsPrimary)
VALUES
    (@OrderId, 1, 'Mobile', '+989121234567', 1),
    (@OrderId, 2, 'Email',  'ali.rezaei@example.ir', 1);
SET IDENTITY_INSERT Ordering.ContactPoints OFF;

INSERT INTO Ordering.JourneyElements
    (OrderId, JourneyElementId, MarketingCarrier, OperatingCarrier, FlightNumber, Origin, Destination,
     DepartureUtc, DepartureLocalDate, DepartureLocalTime, OriginTimeZoneId,
     ArrivalUtc, ArrivalLocalDate, ArrivalLocalTime, DestinationTimeZoneId,
     Cabin, Rbd, MarriedGroup, SequenceInJourney)
VALUES
    (@OrderId, 'JE000001', 'W5', 'W5', '1084', 'THR', 'MHD',
     '2026-09-02T04:30:00', '2026-09-02', '08:00:00', 'Asia/Tehran',
     '2026-09-02T05:55:00', '2026-09-02', '09:25:00', 'Asia/Tehran',
     'Y', 'Q', 'MG01', 1),
    (@OrderId, 'JE000002', 'W5', 'W5', '1085', 'MHD', 'THR',
     '2026-09-09T14:15:00', '2026-09-09', '17:45:00', 'Asia/Tehran',
     '2026-09-09T15:40:00', '2026-09-09', '19:10:00', 'Asia/Tehran',
     'Y', 'Q', 'MG01', 2);

INSERT INTO Ordering.TravelerJourneySegments
    (OrderId, TravelerId, JourneyElementId, SegmentStatus, DeliveryStatus,
     InventoryHoldRef, SeatNumber, SeatAssignedAtUtc, DeliveryRecordNumber, DeliveryUnitNumber, MirroredAtUtc)
VALUES
    (@OrderId, 'TR000001', 'JE000001', 'Confirmed', 'NotStarted', 'HOLD-W5-1084-A', '12A', '2026-08-07T09:05:00', NULL, NULL, NULL),
    (@OrderId, 'TR000001', 'JE000002', 'Confirmed', 'NotStarted', 'HOLD-W5-1085-A', '14C', '2026-08-07T09:05:00', NULL, NULL, NULL),
    (@OrderId, 'TR000002', 'JE000001', 'Confirmed', 'NotStarted', 'HOLD-W5-1084-B', '12B', '2026-08-07T09:05:00', NULL, NULL, NULL),
    (@OrderId, 'TR000002', 'JE000002', 'Confirmed', 'NotStarted', 'HOLD-W5-1085-B', '14D', '2026-08-07T09:05:00', NULL, NULL, NULL);

INSERT INTO Ordering.OrderItems
    (OrderId, OrderItemId, Type, Status, TravelerId, JourneyRefs,
     ProductCode, ProductType, ProductSourceRef,
     PriceTotal, PriceTotalCurrency, PriceCurrency,
     FxFrom, FxTo, FxRate, FxSource, FxCapturedAtUtc,
     PricedAtUtc, PricingEngineVersion,
     OfferId, OfferItemId, OfferExpiryUtc, OfferOwner,
     ReplacesItem,
     PartnerDeliveryOwner, PartnerCommercialOwner, PartnerServicingAuthority,
     PartnerExternalOrderReference, PartnerExternalItemReference, PartnerStatus, PartnerStatusMappedTo,
     PartnerControlTransferState, PartnerRefundResponsibility, PartnerDisruptionResponsibility,
     PartnerSettlementBoundary, PartnerReconciliationReference,
     CancellationReason, CreditAuthorityRef, CreditAuthorityGrantedAtUtc)
VALUES
    (@OrderId, 'OI000001', 'Flight', 'PendingPayment', 'TR000001', 'JE000001,JE000002',
     'FARE-W5-Y-ECOFLEX', 'Flight', 'AIRPRICE-QUOTE-99001',
     28000000.0000, 'IRR', 'IRR', NULL, NULL, NULL, NULL, NULL,
     '2026-08-07T08:58:00', 'airprice-3.4.1',
     'OFFER-2026-08-07-AA31', 'OFFERITEM-1', '2026-08-07T09:30:00', 'AirOffer',
     NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL),

    (@OrderId, 'OI000002', 'Flight', 'PendingPayment', 'TR000002', 'JE000001,JE000002',
     'FARE-W5-Y-ECOFLEX', 'Flight', 'AIRPRICE-QUOTE-99001',
     28000000.0000, 'IRR', 'IRR', NULL, NULL, NULL, NULL, NULL,
     '2026-08-07T08:58:00', 'airprice-3.4.1',
     'OFFER-2026-08-07-AA31', 'OFFERITEM-2', '2026-08-07T09:30:00', 'AirOffer',
     NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL),

    (@OrderId, 'OI000003', 'Bag', 'PendingPayment', 'TR000001', 'JE000001',
     'ANC-BAG-20KG', 'Bag', 'AIRPRICE-QUOTE-99002',
     3500000.0000, 'IRR', 'IRR', NULL, NULL, NULL, NULL, NULL,
     '2026-08-07T09:02:00', 'airprice-3.4.1',
     'OFFER-2026-08-07-AA31', 'OFFERITEM-3', '2026-08-07T09:30:00', 'AirOffer',
     NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL),

    (@OrderId, 'OI000004', 'Fee', 'PendingPayment', 'TR000002', 'JE000001',
     'ANC-LOUNGE-THR', 'Fee', 'AIRPRICE-QUOTE-99003',
     1800000.0000, 'IRR', 'IRR', NULL, NULL, NULL, NULL, NULL,
     '2026-08-07T09:02:00', 'airprice-3.4.1',
     'OFFER-2026-08-07-AA31', 'OFFERITEM-4', '2026-08-07T09:30:00', 'AirOffer',
     NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);

SET IDENTITY_INSERT Ordering.ChargeLines ON;
INSERT INTO Ordering.ChargeLines
    (OrderId, OrderItemId, Seq, ChargeType, Code, Description, Amount, Currency, IsRefundable, TaxJurisdiction)
VALUES
    (@OrderId, 'OI000001', 1, 'Base',       'BASE', 'Base fare round trip', 24000000.0000, 'IRR', 1, NULL),
    (@OrderId, 'OI000001', 2, 'Tax',        'I6',   'VAT',                   2400000.0000, 'IRR', 1, 'IR'),
    (@OrderId, 'OI000001', 3, 'CarrierFee', 'YQ',   'Carrier imposed fee',   1600000.0000, 'IRR', 0, NULL),
    (@OrderId, 'OI000002', 4, 'Base',       'BASE', 'Base fare round trip', 24000000.0000, 'IRR', 1, NULL),
    (@OrderId, 'OI000002', 5, 'Tax',        'I6',   'VAT',                   2400000.0000, 'IRR', 1, 'IR'),
    (@OrderId, 'OI000002', 6, 'CarrierFee', 'YQ',   'Carrier imposed fee',   1600000.0000, 'IRR', 0, NULL),
    (@OrderId, 'OI000003', 7, 'Base',       'BAG',  'Checked bag 20kg',      3181818.0000, 'IRR', 1, NULL),
    (@OrderId, 'OI000003', 8, 'Tax',        'I6',   'VAT',                    318182.0000, 'IRR', 1, 'IR'),
    (@OrderId, 'OI000004', 9, 'Base',       'LNG',  'Lounge access THR',     1636364.0000, 'IRR', 0, NULL),
    (@OrderId, 'OI000004',10, 'Tax',        'I6',   'VAT',                    163636.0000, 'IRR', 0, 'IR');
SET IDENTITY_INSERT Ordering.ChargeLines OFF;

SET IDENTITY_INSERT Ordering.ValueAllocations ON;
INSERT INTO Ordering.ValueAllocations
    (OrderId, OrderItemId, Seq, TravelerId, JourneyElementId, Amount, Currency, AllocationVersion, ResidualPolicy)
VALUES
    (@OrderId, 'OI000001', 1, 'TR000001', 'JE000001', 14000000.0000, 'IRR', 'alloc-v2', 'ProportionalToBase'),
    (@OrderId, 'OI000001', 2, 'TR000001', 'JE000002', 14000000.0000, 'IRR', 'alloc-v2', 'ProportionalToBase'),
    (@OrderId, 'OI000002', 3, 'TR000002', 'JE000001', 14000000.0000, 'IRR', 'alloc-v2', 'ProportionalToBase'),
    (@OrderId, 'OI000002', 4, 'TR000002', 'JE000002', 14000000.0000, 'IRR', 'alloc-v2', 'ProportionalToBase');
SET IDENTITY_INSERT Ordering.ValueAllocations OFF;

INSERT INTO Ordering.TimeLimits
    (OrderId, TimeLimitId, Type, DueAtUtc, Status, AppliesTo, ExtensionCount)
VALUES
    (@OrderId, 'TL000001', 'Payment', '2026-08-07T11:00:00', 'Active',
     'OI000001,OI000002,OI000003,OI000004', 0);

INSERT INTO Audit.OrderHistory
    (OrderId, SequenceNo, OccurredAtUtc, CommandName, ActorType, ActorUserId, ActorSellerId,
     ActorBranchId, ActorOfficeId, ActorHasAirlineOverride, ChannelCode, CorrelationId, CausationId,
     ResultingAggregateVersion)
VALUES
    (@OrderId, 1,'2026-08-07T09:00:00','Create','Agent','user-4471','SELLER-DOTAIR-DIRECT','BRANCH-THR-01','OFFICE-THR-IBE',0,'DirectIbe','CORR-BOOK-77120',NULL, 1),
    (@OrderId, 2,'2026-08-07T09:00:10','AddTraveler','Agent','user-4471','SELLER-DOTAIR-DIRECT','BRANCH-THR-01','OFFICE-THR-IBE',0,'DirectIbe','CORR-BOOK-77120',NULL, 2),
    (@OrderId, 3,'2026-08-07T09:00:20','AddTraveler','Agent','user-4471','SELLER-DOTAIR-DIRECT','BRANCH-THR-01','OFFICE-THR-IBE',0,'DirectIbe','CORR-BOOK-77120',NULL, 3),
    (@OrderId, 4,'2026-08-07T09:00:30','AddJourneyElement','Agent','user-4471','SELLER-DOTAIR-DIRECT','BRANCH-THR-01','OFFICE-THR-IBE',0,'DirectIbe','CORR-BOOK-77120',NULL, 4),
    (@OrderId, 5,'2026-08-07T09:00:40','AddJourneyElement','Agent','user-4471','SELLER-DOTAIR-DIRECT','BRANCH-THR-01','OFFICE-THR-IBE',0,'DirectIbe','CORR-BOOK-77120',NULL, 5),
    (@OrderId, 6,'2026-08-07T09:00:50','AddSegment','Agent','user-4471','SELLER-DOTAIR-DIRECT','BRANCH-THR-01','OFFICE-THR-IBE',0,'DirectIbe','CORR-BOOK-77120',NULL, 6),
    (@OrderId, 7,'2026-08-07T09:01:00','AddSegment','Agent','user-4471','SELLER-DOTAIR-DIRECT','BRANCH-THR-01','OFFICE-THR-IBE',0,'DirectIbe','CORR-BOOK-77120',NULL, 7),
    (@OrderId, 8,'2026-08-07T09:01:10','AddSegment','Agent','user-4471','SELLER-DOTAIR-DIRECT','BRANCH-THR-01','OFFICE-THR-IBE',0,'DirectIbe','CORR-BOOK-77120',NULL, 8),
    (@OrderId, 9,'2026-08-07T09:01:20','AddSegment','Agent','user-4471','SELLER-DOTAIR-DIRECT','BRANCH-THR-01','OFFICE-THR-IBE',0,'DirectIbe','CORR-BOOK-77120',NULL, 9),
    (@OrderId,10,'2026-08-07T09:01:30','AddOrderItem','Agent','user-4471','SELLER-DOTAIR-DIRECT','BRANCH-THR-01','OFFICE-THR-IBE',0,'DirectIbe','CORR-BOOK-77120',NULL,10),
    (@OrderId,11,'2026-08-07T09:01:40','AddOrderItem','Agent','user-4471','SELLER-DOTAIR-DIRECT','BRANCH-THR-01','OFFICE-THR-IBE',0,'DirectIbe','CORR-BOOK-77120',NULL,11),
    (@OrderId,12,'2026-08-07T09:05:00','Confirm','Agent','user-4471','SELLER-DOTAIR-DIRECT','BRANCH-THR-01','OFFICE-THR-IBE',0,'DirectIbe','CORR-BOOK-77120',NULL,12);

INSERT INTO Ops.Operations
    (Id, Type, Status, OrderId, GroupBookingId, CurrentStep, ExpectedExternalMessage,
     CorrelationId, CausationId, AttemptCount, NextRetryAtUtc, TimeoutAtUtc,
     CompensationPolicy, Result, FailureReason, IdempotencyKey, ScopedItems, LastUpdateTime, LastUpdatedBy)
VALUES
    ('0195f3a0-3333-7000-8000-000000000001','Booking','Completed', @OrderId, NULL,'set-time-limits',NULL,
     'CORR-BOOK-77120',NULL,1,NULL,NULL,'Compensate','confirmed',NULL,'booking:K7QM24','OI000001,OI000002',
     '2026-08-07T09:05:00+00:00',NULL);

SET IDENTITY_INSERT Ops.OperationSteps ON;
INSERT INTO Ops.OperationSteps (OperationId, Seq, Name, EnteredAtUtc, CompletedAtUtc, Outcome, Detail)
VALUES
    ('0195f3a0-3333-7000-8000-000000000001',1,'hold-inventory','2026-08-07T09:00:00','2026-08-07T09:01:00','ok','4 seats held'),
    ('0195f3a0-3333-7000-8000-000000000001',2,'create-order','2026-08-07T09:01:00','2026-08-07T09:02:00','ok',NULL),
    ('0195f3a0-3333-7000-8000-000000000001',3,'confirm-inventory','2026-08-07T09:02:00','2026-08-07T09:04:00','ok',NULL),
    ('0195f3a0-3333-7000-8000-000000000001',4,'set-time-limits','2026-08-07T09:04:00','2026-08-07T09:05:00','ok','TL000001');
SET IDENTITY_INSERT Ops.OperationSteps OFF;

COMMIT TRANSACTION;
PRINT '01-order-created: K7QM24 Pending, 2 travellers, 2 flights, 4 items';
