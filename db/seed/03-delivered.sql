/*  03-delivered.sql
    Delivery operation: one DeliveryRecord per traveller, numbers drawn from the
    branch range, tax invoices raised and accepted.

    Ancillary kinds, per COMPLETE-DOMAIN-REFERENCE section 3.1.3:
      unit 3 of IR-THR01-1000001  AssociatedSegmentUnit = 1     associated  (IATA EMD-A equivalent)
      unit 3 of IR-THR01-1000002  AssociatedSegmentUnit = NULL  standalone  (IATA EMD-S equivalent)

    I-30  four numbers consumed -> NextAvailable = RangeStart + 4
    I-33  VoidWindowHours snapshotted on each record at issuance
    I-34  segment unit values sum to the item value allocation

    Units are left Open here; travel happens in 05-flown.sql.
    Depends on: 02-payment-settled.sql
*/
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;
BEGIN TRANSACTION;

DECLARE @OrderId uniqueidentifier = '0195f3a0-1111-7000-8000-000000000001';

INSERT INTO Delivery.DocumentNumberRanges
    (RangeId, BranchId, DocumentKind, Prefix, RangeStart, RangeEnd, NextAvailable, Status,
     RegisteredWithAuthorityAtUtc, AuthorityReference, LastUpdateTime, LastUpdatedBy)
VALUES
    ('0195f3a0-2222-7000-8000-000000000001', 'BRANCH-THR-01', 'Invoice', 'IR-THR01-',
     1000001, 1999999, 1000005, 'Active',
     '2026-01-15T00:00:00', 'INTA-REG-THR01-2026', '2026-08-07T09:15:00+00:00', NULL);

INSERT INTO Delivery.DeliveryRecords
    (DeliveryRecordNumber, OrderId, TravelerId,
     Given, Surname, Title, GivenLocal, SurnameLocal, Ptc, DateOfBirth,
     DocumentType, DocumentNumber, DocumentIssuingCountry, DocumentExpiry, DocumentNationality,
     BranchId, IssuedAtUtc, VoidWindowHours, AggregateVersion, LastUpdateTime, LastUpdatedBy)
VALUES
    ('IR-THR01-1000001', @OrderId, 'TR000001',
     'ALI', 'REZAEI', 'MR', N'علی', N'رضایی', 'ADT', '1988-04-12',
     'NationalId', '0064328871', 'IR', NULL, 'IR',
     'BRANCH-THR-01', '2026-08-07T09:15:00', 24, 1, '2026-08-07T09:15:00+00:00', NULL),

    ('IR-THR01-1000002', @OrderId, 'TR000002',
     'SARA', 'MORADI', 'MS', N'سارا', N'مرادی', 'ADT', '1991-11-03',
     'NationalId', '0079914452', 'IR', NULL, 'IR',
     'BRANCH-THR-01', '2026-08-07T09:15:00', 24, 1, '2026-08-07T09:15:00+00:00', NULL);

INSERT INTO Delivery.SegmentDeliveries
    (DeliveryRecordNumber, UnitNumber, OrderItemId, JourneyElementId,
     MarketingCarrier, OperatingCarrier, FlightNumber, Origin, Destination,
     DepartureUtc, DepartureLocalDate, DepartureLocalTime, OriginTimeZoneId,
     ArrivalUtc, ArrivalLocalDate, ArrivalLocalTime, DestinationTimeZoneId,
     Status, ControlHolder, ControlAcquiredAtUtc, ControlLeaseExpiresAtUtc, UnitValue, Currency)
VALUES
    ('IR-THR01-1000001', 1, 'OI000001', 'JE000001',
     'W5','W5','1084','THR','MHD',
     '2026-09-02T04:30:00','2026-09-02','08:00:00','Asia/Tehran',
     '2026-09-02T05:55:00','2026-09-02','09:25:00','Asia/Tehran',
     'Open', NULL, NULL, NULL, 14000000.0000, 'IRR'),

    ('IR-THR01-1000001', 2, 'OI000001', 'JE000002',
     'W5','W5','1085','MHD','THR',
     '2026-09-09T14:15:00','2026-09-09','17:45:00','Asia/Tehran',
     '2026-09-09T15:40:00','2026-09-09','19:10:00','Asia/Tehran',
     'Open', NULL, NULL, NULL, 14000000.0000, 'IRR'),

    ('IR-THR01-1000002', 1, 'OI000002', 'JE000001',
     'W5','W5','1084','THR','MHD',
     '2026-09-02T04:30:00','2026-09-02','08:00:00','Asia/Tehran',
     '2026-09-02T05:55:00','2026-09-02','09:25:00','Asia/Tehran',
     'Open', NULL, NULL, NULL, 14000000.0000, 'IRR'),

    ('IR-THR01-1000002', 2, 'OI000002', 'JE000002',
     'W5','W5','1085','MHD','THR',
     '2026-09-09T14:15:00','2026-09-09','17:45:00','Asia/Tehran',
     '2026-09-09T15:40:00','2026-09-09','19:10:00','Asia/Tehran',
     'Open', NULL, NULL, NULL, 14000000.0000, 'IRR');

INSERT INTO Delivery.ServiceDeliveries
    (DeliveryRecordNumber, UnitNumber, OrderItemId, ServiceCode, Description, Status, UnitValue, Currency, AssociatedSegmentUnit)
VALUES
    ('IR-THR01-1000001', 3, 'OI000003', 'BAG20',  'Checked bag 20kg THR-MHD', 'Open', 3500000.0000, 'IRR', 1),
    ('IR-THR01-1000002', 3, 'OI000004', 'LOUNGE', 'Lounge access THR',        'Open', 1800000.0000, 'IRR', NULL);

UPDATE Ordering.OrderItems SET Status = 'Delivered'
WHERE OrderId = @OrderId AND Status = 'Confirmed';

UPDATE s SET DeliveryRecordNumber = d.DeliveryRecordNumber,
             DeliveryUnitNumber   = CASE WHEN s.JourneyElementId = 'JE000001' THEN 1 ELSE 2 END,
             MirroredAtUtc        = '2026-08-07T09:15:00'
FROM Ordering.TravelerJourneySegments s
JOIN Delivery.DeliveryRecords d ON d.OrderId = s.OrderId AND d.TravelerId = s.TravelerId
WHERE s.OrderId = @OrderId;

INSERT INTO Delivery.TaxDocuments
    (DocumentNumber, Kind, OrderId, DeliveryRecordNumber, ReversesDocumentNumber, BranchId,
     SpecVersion, BuyerTaxIdentity, PassengerIdentity, IssuedAtUtc,
     TotalAmount, Currency, TotalTax, TotalTaxCurrency, SubmissionStatus,
     AggregateVersion, LastUpdateTime, LastUpdatedBy)
VALUES
    ('IR-THR01-1000003', 'Invoice', @OrderId, 'IR-THR01-1000001', NULL, 'BRANCH-THR-01',
     'INTA-2025.1', NULL, '0064328871', '2026-08-07T09:15:00',
     31500000.0000, 'IRR', 2718182.0000, 'IRR', 'Accepted', 3, '2026-08-07T10:05:00+00:00', NULL),

    ('IR-THR01-1000004', 'Invoice', @OrderId, 'IR-THR01-1000002', NULL, 'BRANCH-THR-01',
     'INTA-2025.1', NULL, '0079914452', '2026-08-07T09:15:00',
     29800000.0000, 'IRR', 2563636.0000, 'IRR', 'Accepted', 3, '2026-08-07T10:05:00+00:00', NULL);

INSERT INTO Delivery.TaxDocumentLines
    (DocumentNumber, Seq, LineType, Code, Description, NetAmount, Currency, TaxAmount, TaxCurrency, TaxRate, TaxJurisdiction)
VALUES
    ('IR-THR01-1000003', 1, 'Transport', 'BASE',   'Air transport THR-MHD-THR', 25600000.0000, 'IRR', 2400000.0000, 'IRR', 0.100000, 'IR'),
    ('IR-THR01-1000003', 2, 'Ancillary', 'BAG20',  'Checked bag 20kg',           3181818.0000, 'IRR',  318182.0000, 'IRR', 0.100000, 'IR'),
    ('IR-THR01-1000004', 3, 'Transport', 'BASE',   'Air transport THR-MHD-THR', 25600000.0000, 'IRR', 2400000.0000, 'IRR', 0.100000, 'IR'),
    ('IR-THR01-1000004', 4, 'Ancillary', 'LOUNGE', 'Lounge access THR',          1636364.0000, 'IRR',  163636.0000, 'IRR', 0.100000, 'IR');

INSERT INTO Delivery.TaxDocumentSubmissionAttempts
    (DocumentNumber, Seq, AttemptedAtUtc, Outcome, ResponseReference, RejectionReason)
VALUES
    ('IR-THR01-1000003', 1, '2026-08-07T09:40:00', 'Submitted', 'INTA-ACK-551201', NULL),
    ('IR-THR01-1000003', 2, '2026-08-07T10:05:00', 'Accepted',  'INTA-ACK-551201', NULL),
    ('IR-THR01-1000004', 3, '2026-08-07T09:40:00', 'Submitted', 'INTA-ACK-551202', NULL),
    ('IR-THR01-1000004', 4, '2026-08-07T10:05:00', 'Accepted',  'INTA-ACK-551202', NULL);

UPDATE Ordering.Orders
SET AggregateVersion = 16, LastUpdateTime = '2026-08-07T09:15:00+00:00'
WHERE Id = @OrderId;

INSERT INTO Audit.OrderHistory
    (OrderId, SequenceNo, OccurredAtUtc, CommandName, ActorType, ActorUserId, ActorSellerId,
     ActorBranchId, ActorOfficeId, ActorHasAirlineOverride, ChannelCode, CorrelationId, CausationId,
     ResultingAggregateVersion)
VALUES
    (@OrderId,16,'2026-08-07T09:15:00','ApplyDeliveryResult','System','worker-delivery',NULL,NULL,NULL,0,'DirectIbe','CORR-DEL-77120','CORR-PAY-77120',16);

INSERT INTO Ops.Operations
    (Id, Type, Status, OrderId, GroupBookingId, CurrentStep, ExpectedExternalMessage,
     CorrelationId, CausationId, AttemptCount, NextRetryAtUtc, TimeoutAtUtc,
     CompensationPolicy, Result, FailureReason, IdempotencyKey, ScopedItems, LastUpdateTime, LastUpdatedBy)
VALUES
    ('0195f3a0-3333-7000-8000-000000000003','Delivery','Completed', @OrderId, NULL,'queue-submission',NULL,
     'CORR-DEL-77120','CORR-PAY-77120',1,NULL,NULL,'ForwardOnly','2 delivery records issued',NULL,'delivery:K7QM24',
     'OI000001,OI000002,OI000003,OI000004','2026-08-07T09:15:00+00:00',NULL),

    ('0195f3a0-3333-7000-8000-000000000004','TaxSubmission','Completed', @OrderId, NULL,'accept',NULL,
     'CORR-TAX-77120','CORR-DEL-77120',1,NULL,NULL,'ForwardOnly','2 documents accepted',NULL,'tax:K7QM24','',
     '2026-08-07T10:05:00+00:00',NULL);

SET IDENTITY_INSERT Ops.OperationSteps ON;
INSERT INTO Ops.OperationSteps (OperationId, Seq, Name, EnteredAtUtc, CompletedAtUtc, Outcome, Detail)
VALUES
    ('0195f3a0-3333-7000-8000-000000000003', 8,'check-eligibility','2026-08-07T09:13:00','2026-08-07T09:13:30','ok',NULL),
    ('0195f3a0-3333-7000-8000-000000000003', 9,'create-delivery-records','2026-08-07T09:13:30','2026-08-07T09:14:00','ok','2 records'),
    ('0195f3a0-3333-7000-8000-000000000003',10,'allocate-number','2026-08-07T09:14:00','2026-08-07T09:14:30','ok','1000001,1000002'),
    ('0195f3a0-3333-7000-8000-000000000003',11,'raise-tax-document','2026-08-07T09:14:30','2026-08-07T09:15:00','ok','1000003,1000004'),
    ('0195f3a0-3333-7000-8000-000000000003',12,'queue-submission','2026-08-07T09:15:00','2026-08-07T09:15:10','ok',NULL),
    ('0195f3a0-3333-7000-8000-000000000004',13,'submit','2026-08-07T09:40:00','2026-08-07T09:41:00','ok',NULL),
    ('0195f3a0-3333-7000-8000-000000000004',14,'await-response','2026-08-07T09:41:00','2026-08-07T10:04:00','ok',NULL),
    ('0195f3a0-3333-7000-8000-000000000004',15,'accept','2026-08-07T10:04:00','2026-08-07T10:05:00','ok','both accepted');
SET IDENTITY_INSERT Ops.OperationSteps OFF;

COMMIT TRANSACTION;
PRINT '03-delivered: 2 delivery records, 2 invoices accepted, 4 numbers consumed';
