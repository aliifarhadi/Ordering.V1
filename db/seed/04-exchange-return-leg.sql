/*  04-exchange-return-leg.sql
    Sara Moradi (TR000002) moves her return leg from W5-1085 on 9 Sep
    to W5-1087 on 12 Sep. Ali is unaffected.

    How the exchange is modelled, and why:

    1. The original priced item is never mutated (I-12). A NEW item OI000005 is
       created carrying the new price, and its ReplacesItem points at OI000002.
       The original moves to Exchanged.
    2. A NEW journey element JE000003 holds the new flight snapshot. The original
       JE000002 stays, because Ali still travels on it - a journey element is
       order-level, not per traveller.
    3. Sara's segment is repointed from JE000002 to JE000003. Confirm the new
       segment before releasing the old (rebooking rule, reference section 2.1.12).
    4. The delivery unit for the old leg moves Open -> Exchanged, which is legal
       ONLY while the unit is Open. Once Flown it can only be refunded. This is
       why the exchange runs before 05-flown.sql.
    5. Fare difference 2,400,000 IRR collected as a second payment record.
    6. A credit note reverses the original invoice and a new invoice is issued
       (I-32: a reversal references the document it reverses).

    Depends on: 03-delivered.sql   Must run BEFORE 05-flown.sql.
*/
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;
BEGIN TRANSACTION;

DECLARE @OrderId uniqueidentifier = '0195f3a0-1111-7000-8000-000000000001';

/* --- 1. new journey element for the replacement flight --- */
INSERT INTO Ordering.JourneyElements
    (OrderId, JourneyElementId, MarketingCarrier, OperatingCarrier, FlightNumber, Origin, Destination,
     DepartureUtc, DepartureLocalDate, DepartureLocalTime, OriginTimeZoneId,
     ArrivalUtc, ArrivalLocalDate, ArrivalLocalTime, DestinationTimeZoneId,
     Cabin, Rbd, MarriedGroup, SequenceInJourney)
VALUES
    (@OrderId, 'JE000003', 'W5', 'W5', '1087', 'MHD', 'THR',
     '2026-09-12T10:45:00', '2026-09-12', '14:15:00', 'Asia/Tehran',
     '2026-09-12T12:10:00', '2026-09-12', '15:40:00', 'Asia/Tehran',
     'Y', 'M', NULL, 2);

/* --- 2. confirm the new segment BEFORE releasing the old one --- */
INSERT INTO Ordering.TravelerJourneySegments
    (OrderId, TravelerId, JourneyElementId, SegmentStatus, DeliveryStatus,
     InventoryHoldRef, SeatNumber, SeatAssignedAtUtc, DeliveryRecordNumber, DeliveryUnitNumber, MirroredAtUtc)
VALUES
    (@OrderId, 'TR000002', 'JE000003', 'Confirmed', 'NotStarted', 'HOLD-W5-1087-B', '9F', '2026-08-20T11:05:00', NULL, NULL, NULL);

UPDATE Ordering.TravelerJourneySegments
SET SegmentStatus = 'Rebooked'
WHERE OrderId = @OrderId AND TravelerId = 'TR000002' AND JourneyElementId = 'JE000002';

/* --- 3. replacement item; original is never repriced (I-12) --- */
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
    (@OrderId, 'OI000005', 'Flight', 'Delivered', 'TR000002', 'JE000001,JE000003',
     'FARE-W5-Y-ECOSTD', 'Flight', 'AIRPRICE-QUOTE-99101',
     30400000.0000, 'IRR', 'IRR', NULL, NULL, NULL, NULL, NULL,
     '2026-08-20T11:00:00', 'airprice-3.4.1',
     'OFFER-2026-08-20-BB77', 'OFFERITEM-1', '2026-08-20T12:00:00', 'AirOffer',
     'OI000002',
     NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);

UPDATE Ordering.OrderItems SET Status = 'Exchanged'
WHERE OrderId = @OrderId AND OrderItemId = 'OI000002';

/* --- 4. price breakdown for the replacement item: 26,000,000 + 2,600,000 + 1,800,000 = 30,400,000 --- */
SET IDENTITY_INSERT Ordering.ChargeLines ON;
INSERT INTO Ordering.ChargeLines
    (OrderId, OrderItemId, Seq, ChargeType, Code, Description, Amount, Currency, IsRefundable, TaxJurisdiction)
VALUES
    (@OrderId, 'OI000005', 11, 'Base',       'BASE', 'Base fare THR-MHD / MHD-THR reissue', 26000000.0000, 'IRR', 1, NULL),
    (@OrderId, 'OI000005', 12, 'Tax',        'I6',   'VAT',                                  2600000.0000, 'IRR', 1, 'IR'),
    (@OrderId, 'OI000005', 13, 'CarrierFee', 'YQ',   'Carrier imposed fee',                  1800000.0000, 'IRR', 0, NULL);
SET IDENTITY_INSERT Ordering.ChargeLines OFF;

SET IDENTITY_INSERT Ordering.ValueAllocations ON;
INSERT INTO Ordering.ValueAllocations
    (OrderId, OrderItemId, Seq, TravelerId, JourneyElementId, Amount, Currency, AllocationVersion, ResidualPolicy)
VALUES
    (@OrderId, 'OI000005', 5, 'TR000002', 'JE000001', 14000000.0000, 'IRR', 'alloc-v3', 'SuppliedExplicitly'),
    (@OrderId, 'OI000005', 6, 'TR000002', 'JE000003', 16400000.0000, 'IRR', 'alloc-v3', 'SuppliedExplicitly');
SET IDENTITY_INSERT Ordering.ValueAllocations OFF;

/* --- 5. fare difference 30,400,000 - 28,000,000 = 2,400,000 --- */
INSERT INTO Ordering.PaymentRecords
    (OrderId, PaymentRecordId, FormOfPayment, Amount, Currency, Status, PaymentRequestRef,
     FxFrom, FxTo, FxRate, FxSource, FxCapturedAtUtc, PayerRef, RecordedAtUtc, IsUnderDispute)
VALUES
    (@OrderId, 'PR000002', 'Card', 2400000.0000, 'IRR', 'Settled', 'PAYREQ-2026-0820-77301',
     NULL, NULL, NULL, NULL, NULL, 'PAYER-SARA-MORADI', '2026-08-20T11:10:00', 0);

INSERT INTO Ordering.PaymentAllocations
    (OrderId, PaymentRecordId, OrderItemId, Amount, Currency)
VALUES
    (@OrderId, 'PR000002', 'OI000005', 2400000.0000, 'IRR');

/*  The original 28,000,000 allocated to OI000002 is carried onto the replacement
    item, so the exchanged item holds no allocation and OI000005 is fully covered. */
UPDATE Ordering.PaymentAllocations
SET OrderItemId = 'OI000005'
WHERE OrderId = @OrderId AND PaymentRecordId = 'PR000001' AND OrderItemId = 'OI000002';

/* --- 6. delivery side: old unit Exchanged (legal only while Open), new unit issued --- */
UPDATE Delivery.SegmentDeliveries
SET Status = 'Exchanged'
WHERE DeliveryRecordNumber = 'IR-THR01-1000002' AND UnitNumber = 2;

INSERT INTO Delivery.SegmentDeliveryStatusHistory
    (DeliveryRecordNumber, UnitNumber, Seq, FromStatus, ToStatus, OccurredAtUtc, Source)
VALUES
    ('IR-THR01-1000002', 2, 1, 'Open', 'Exchanged', '2026-08-20T11:15:00', 'AGENT-user-4471');

INSERT INTO Delivery.SegmentDeliveries
    (DeliveryRecordNumber, UnitNumber, OrderItemId, JourneyElementId,
     MarketingCarrier, OperatingCarrier, FlightNumber, Origin, Destination,
     DepartureUtc, DepartureLocalDate, DepartureLocalTime, OriginTimeZoneId,
     ArrivalUtc, ArrivalLocalDate, ArrivalLocalTime, DestinationTimeZoneId,
     Status, ControlHolder, ControlAcquiredAtUtc, ControlLeaseExpiresAtUtc, UnitValue, Currency)
VALUES
    ('IR-THR01-1000002', 4, 'OI000005', 'JE000003',
     'W5','W5','1087','MHD','THR',
     '2026-09-12T10:45:00','2026-09-12','14:15:00','Asia/Tehran',
     '2026-09-12T12:10:00','2026-09-12','15:40:00','Asia/Tehran',
     'Open', NULL, NULL, NULL, 16400000.0000, 'IRR');

/*  Unit 1 still belongs to the exchanged item OI000002; repoint it at the
    replacement item so both of Sara's live units reference OI000005. */
UPDATE Delivery.SegmentDeliveries
SET OrderItemId = 'OI000005'
WHERE DeliveryRecordNumber = 'IR-THR01-1000002' AND UnitNumber = 1;

UPDATE Ordering.TravelerJourneySegments
SET DeliveryRecordNumber = 'IR-THR01-1000002', DeliveryUnitNumber = 4, MirroredAtUtc = '2026-08-20T11:15:00'
WHERE OrderId = @OrderId AND TravelerId = 'TR000002' AND JourneyElementId = 'JE000003';

UPDATE Ordering.TravelerJourneySegments
SET DeliveryRecordNumber = NULL, DeliveryUnitNumber = NULL, MirroredAtUtc = '2026-08-20T11:15:00'
WHERE OrderId = @OrderId AND TravelerId = 'TR000002' AND JourneyElementId = 'JE000002';

UPDATE Delivery.DeliveryRecords
SET AggregateVersion = 3, LastUpdateTime = '2026-08-20T11:15:00+00:00'
WHERE DeliveryRecordNumber = 'IR-THR01-1000002';

/* --- 7. tax: credit note reverses the original invoice, new invoice issued (I-32) --- */
INSERT INTO Delivery.TaxDocuments
    (DocumentNumber, Kind, OrderId, DeliveryRecordNumber, ReversesDocumentNumber, BranchId,
     SpecVersion, BuyerTaxIdentity, PassengerIdentity, IssuedAtUtc,
     TotalAmount, Currency, TotalTax, TotalTaxCurrency, SubmissionStatus,
     AggregateVersion, LastUpdateTime, LastUpdatedBy)
VALUES
    ('IR-THR01-1000005', 'CreditNote', @OrderId, 'IR-THR01-1000002', 'IR-THR01-1000004', 'BRANCH-THR-01',
     'INTA-2025.1', NULL, '0079914452', '2026-08-20T11:20:00',
     -29800000.0000, 'IRR', -2563636.0000, 'IRR', 'Accepted', 3, '2026-08-20T11:45:00+00:00', NULL),

    ('IR-THR01-1000006', 'Invoice', @OrderId, 'IR-THR01-1000002', NULL, 'BRANCH-THR-01',
     'INTA-2025.1', NULL, '0079914452', '2026-08-20T11:20:00',
     32200000.0000, 'IRR', 2763636.0000, 'IRR', 'Accepted', 3, '2026-08-20T11:45:00+00:00', NULL);

INSERT INTO Delivery.TaxDocumentLines
    (DocumentNumber, Seq, LineType, Code, Description, NetAmount, Currency, TaxAmount, TaxCurrency, TaxRate, TaxJurisdiction)
VALUES
    ('IR-THR01-1000005', 5, 'Transport', 'BASE',   'Reversal air transport THR-MHD-THR', -25600000.0000, 'IRR', -2400000.0000, 'IRR', 0.100000, 'IR'),
    ('IR-THR01-1000005', 6, 'Ancillary', 'LOUNGE', 'Reversal lounge access THR',          -1636364.0000, 'IRR',  -163636.0000, 'IRR', 0.100000, 'IR'),
    ('IR-THR01-1000006', 7, 'Transport', 'BASE',   'Air transport THR-MHD / MHD-THR reissue', 27800000.0000, 'IRR', 2600000.0000, 'IRR', 0.100000, 'IR'),
    ('IR-THR01-1000006', 8, 'Ancillary', 'LOUNGE', 'Lounge access THR',                        1636364.0000, 'IRR',  163636.0000, 'IRR', 0.100000, 'IR');

INSERT INTO Delivery.TaxDocumentSubmissionAttempts
    (DocumentNumber, Seq, AttemptedAtUtc, Outcome, ResponseReference, RejectionReason)
VALUES
    ('IR-THR01-1000005', 5, '2026-08-20T11:30:00', 'Submitted', 'INTA-ACK-551455', NULL),
    ('IR-THR01-1000005', 6, '2026-08-20T11:45:00', 'Accepted',  'INTA-ACK-551455', NULL),
    ('IR-THR01-1000006', 7, '2026-08-20T11:30:00', 'Submitted', 'INTA-ACK-551456', NULL),
    ('IR-THR01-1000006', 8, '2026-08-20T11:45:00', 'Accepted',  'INTA-ACK-551456', NULL);

UPDATE Delivery.DocumentNumberRanges
SET NextAvailable = 1000007, LastUpdateTime = '2026-08-20T11:20:00+00:00'
WHERE RangeId = '0195f3a0-2222-7000-8000-000000000001';

/* --- 8. audit and operation --- */
UPDATE Ordering.Orders
SET AggregateVersion = 20, LastUpdateTime = '2026-08-20T11:20:00+00:00'
WHERE Id = @OrderId;

INSERT INTO Audit.OrderHistory
    (OrderId, SequenceNo, OccurredAtUtc, CommandName, ActorType, ActorUserId, ActorSellerId,
     ActorBranchId, ActorOfficeId, ActorHasAirlineOverride, ChannelCode, CorrelationId, CausationId,
     ResultingAggregateVersion)
VALUES
    (@OrderId,17,'2026-08-20T11:00:00','AddJourneyElement','Agent','user-4471','SELLER-DOTAIR-DIRECT','BRANCH-THR-01','OFFICE-THR-IBE',0,'BackOffice','CORR-EXCH-77301',NULL,17),
    (@OrderId,18,'2026-08-20T11:05:00','AddSegment','Agent','user-4471','SELLER-DOTAIR-DIRECT','BRANCH-THR-01','OFFICE-THR-IBE',0,'BackOffice','CORR-EXCH-77301',NULL,18),
    (@OrderId,19,'2026-08-20T11:08:00','AddOrderItem','Agent','user-4471','SELLER-DOTAIR-DIRECT','BRANCH-THR-01','OFFICE-THR-IBE',0,'BackOffice','CORR-EXCH-77301',NULL,19),
    (@OrderId,20,'2026-08-20T11:15:00','RebookJourneyElement','Agent','user-4471','SELLER-DOTAIR-DIRECT','BRANCH-THR-01','OFFICE-THR-IBE',0,'BackOffice','CORR-EXCH-77301',NULL,20);

INSERT INTO Ops.Operations
    (Id, Type, Status, OrderId, GroupBookingId, CurrentStep, ExpectedExternalMessage,
     CorrelationId, CausationId, AttemptCount, NextRetryAtUtc, TimeoutAtUtc,
     CompensationPolicy, Result, FailureReason, IdempotencyKey, ScopedItems, LastUpdateTime, LastUpdatedBy)
VALUES
    ('0195f3a0-3333-7000-8000-000000000005','Cancellation','Completed', @OrderId, NULL,'reissue-complete',NULL,
     'CORR-EXCH-77301',NULL,1,NULL,NULL,'ForwardOnly','OI000002 exchanged for OI000005',NULL,
     'exchange:K7QM24:TR000002','OI000002,OI000005','2026-08-20T11:20:00+00:00',NULL);

SET IDENTITY_INSERT Ops.OperationSteps ON;
INSERT INTO Ops.OperationSteps (OperationId, Seq, Name, EnteredAtUtc, CompletedAtUtc, Outcome, Detail)
VALUES
    ('0195f3a0-3333-7000-8000-000000000005',16,'authority-and-control-check','2026-08-20T11:00:00','2026-08-20T11:00:30','ok','no unit under external control'),
    ('0195f3a0-3333-7000-8000-000000000005',17,'price-replacement','2026-08-20T11:00:30','2026-08-20T11:02:00','ok','30,400,000 IRR'),
    ('0195f3a0-3333-7000-8000-000000000005',18,'confirm-new-inventory','2026-08-20T11:02:00','2026-08-20T11:05:00','ok','W5-1087 12 Sep confirmed'),
    ('0195f3a0-3333-7000-8000-000000000005',19,'collect-fare-difference','2026-08-20T11:05:00','2026-08-20T11:10:00','ok','2,400,000 IRR'),
    ('0195f3a0-3333-7000-8000-000000000005',20,'exchange-delivery-unit','2026-08-20T11:10:00','2026-08-20T11:15:00','ok','unit 2 Exchanged, unit 4 issued'),
    ('0195f3a0-3333-7000-8000-000000000005',21,'release-old-inventory','2026-08-20T11:15:00','2026-08-20T11:16:00','ok','W5-1085 seat 14D released'),
    ('0195f3a0-3333-7000-8000-000000000005',22,'reissue-tax-documents','2026-08-20T11:16:00','2026-08-20T11:20:00','ok','credit note 1000005, invoice 1000006'),
    ('0195f3a0-3333-7000-8000-000000000005',23,'reissue-complete','2026-08-20T11:20:00','2026-08-20T11:20:10','ok',NULL);
SET IDENTITY_INSERT Ops.OperationSteps OFF;

COMMIT TRANSACTION;
PRINT '04-exchange: TR000002 return leg 9 Sep -> 12 Sep; OI000002 Exchanged -> OI000005';
