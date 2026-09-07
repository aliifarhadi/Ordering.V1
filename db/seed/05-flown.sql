/*  05-flown.sql
    Travel happens. Each live segment unit walks the full DCS lifecycle
    Open -> ControlTransferred -> CheckedIn -> Boarded -> Flown, and both
    service units are consumed.

    Ali  flies 2 Sep outbound and 9 Sep return.
    Sara flies 2 Sep outbound and 12 Sep return (her exchanged leg).
    The unit exchanged in step 04 stays Exchanged and never flies.

    After this file:
      Order.Status            Flown           all flight items Used
      DeliveryRecord 1        Used            all segment units Flown, no open service unit
      DeliveryRecord 2        Closed          mixed terminal kinds: Flown + Exchanged

    Depends on: 04-exchange-return-leg.sql
*/
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;
BEGIN TRANSACTION;

DECLARE @OrderId uniqueidentifier = '0195f3a0-1111-7000-8000-000000000001';

/* --- segment units reach Flown --- */
UPDATE Delivery.SegmentDeliveries SET Status = 'Flown', ControlHolder = NULL,
       ControlAcquiredAtUtc = NULL, ControlLeaseExpiresAtUtc = NULL
WHERE (DeliveryRecordNumber = 'IR-THR01-1000001' AND UnitNumber IN (1,2))
   OR (DeliveryRecordNumber = 'IR-THR01-1000002' AND UnitNumber IN (1,4));

INSERT INTO Delivery.SegmentDeliveryStatusHistory
    (DeliveryRecordNumber, UnitNumber, Seq, FromStatus, ToStatus, OccurredAtUtc, Source)
VALUES
    -- Ali outbound 2 Sep
    ('IR-THR01-1000001', 1, 1, 'Open','ControlTransferred','2026-09-02T00:30:00','DCS-THR'),
    ('IR-THR01-1000001', 1, 2, 'ControlTransferred','CheckedIn','2026-09-02T02:10:00','DCS-THR'),
    ('IR-THR01-1000001', 1, 3, 'CheckedIn','Boarded','2026-09-02T04:05:00','DCS-THR'),
    ('IR-THR01-1000001', 1, 4, 'Boarded','Flown','2026-09-02T06:10:00','DCS-THR'),
    -- Ali return 9 Sep
    ('IR-THR01-1000001', 2, 5, 'Open','ControlTransferred','2026-09-09T10:15:00','DCS-MHD'),
    ('IR-THR01-1000001', 2, 6, 'ControlTransferred','CheckedIn','2026-09-09T12:00:00','DCS-MHD'),
    ('IR-THR01-1000001', 2, 7, 'CheckedIn','Boarded','2026-09-09T13:50:00','DCS-MHD'),
    ('IR-THR01-1000001', 2, 8, 'Boarded','Flown','2026-09-09T15:55:00','DCS-MHD'),
    -- Sara outbound 2 Sep
    ('IR-THR01-1000002', 1, 9, 'Open','ControlTransferred','2026-09-02T00:30:00','DCS-THR'),
    ('IR-THR01-1000002', 1,10, 'ControlTransferred','CheckedIn','2026-09-02T02:12:00','DCS-THR'),
    ('IR-THR01-1000002', 1,11, 'CheckedIn','Boarded','2026-09-02T04:06:00','DCS-THR'),
    ('IR-THR01-1000002', 1,12, 'Boarded','Flown','2026-09-02T06:10:00','DCS-THR'),
    -- Sara exchanged return 12 Sep
    ('IR-THR01-1000002', 4,13, 'Open','ControlTransferred','2026-09-12T06:45:00','DCS-MHD'),
    ('IR-THR01-1000002', 4,14, 'ControlTransferred','CheckedIn','2026-09-12T08:30:00','DCS-MHD'),
    ('IR-THR01-1000002', 4,15, 'CheckedIn','Boarded','2026-09-12T10:20:00','DCS-MHD'),
    ('IR-THR01-1000002', 4,16, 'Boarded','Flown','2026-09-12T12:25:00','DCS-MHD');

/* --- service units consumed; ServiceDeliveryConsumed moves its item to Used --- */
UPDATE Delivery.ServiceDeliveries SET Status = 'Consumed'
WHERE DeliveryRecordNumber IN ('IR-THR01-1000001','IR-THR01-1000002') AND UnitNumber = 3;

/* --- mirror onto the order; DeliveryStatus is written only by inbound events (I-22) --- */
UPDATE Ordering.TravelerJourneySegments SET DeliveryStatus = 'Flown', MirroredAtUtc = '2026-09-02T06:10:00'
WHERE OrderId = @OrderId AND JourneyElementId = 'JE000001';

UPDATE Ordering.TravelerJourneySegments SET DeliveryStatus = 'Flown', MirroredAtUtc = '2026-09-09T15:55:00'
WHERE OrderId = @OrderId AND TravelerId = 'TR000001' AND JourneyElementId = 'JE000002';

UPDATE Ordering.TravelerJourneySegments SET DeliveryStatus = 'Flown', MirroredAtUtc = '2026-09-12T12:25:00'
WHERE OrderId = @OrderId AND TravelerId = 'TR000002' AND JourneyElementId = 'JE000003';

/* --- items consumed; the exchanged item stays Exchanged --- */
UPDATE Ordering.OrderItems SET Status = 'Used'
WHERE OrderId = @OrderId AND OrderItemId IN ('OI000001','OI000003','OI000004','OI000005');

UPDATE Delivery.DeliveryRecords SET AggregateVersion = AggregateVersion + 4,
       LastUpdateTime = '2026-09-12T12:25:00+00:00'
WHERE OrderId = @OrderId;

UPDATE Ordering.Orders SET AggregateVersion = 26, LastUpdateTime = '2026-09-12T12:25:00+00:00'
WHERE Id = @OrderId;

INSERT INTO Audit.OrderHistory
    (OrderId, SequenceNo, OccurredAtUtc, CommandName, ActorType, ActorUserId, ActorSellerId,
     ActorBranchId, ActorOfficeId, ActorHasAirlineOverride, ChannelCode, CorrelationId, CausationId,
     ResultingAggregateVersion)
VALUES
    (@OrderId,21,'2026-09-02T02:10:00','ApplyDeliveryStatusChange','System','dcs-bridge',NULL,NULL,NULL,0,'Airport','CORR-DCS-90201',NULL,21),
    (@OrderId,22,'2026-09-02T04:05:00','ApplyDeliveryStatusChange','System','dcs-bridge',NULL,NULL,NULL,0,'Airport','CORR-DCS-90202',NULL,22),
    (@OrderId,23,'2026-09-02T06:10:00','ApplyDeliveryStatusChange','System','dcs-bridge',NULL,NULL,NULL,0,'Airport','CORR-DCS-90203',NULL,23),
    (@OrderId,24,'2026-09-09T15:55:00','ApplyDeliveryStatusChange','System','dcs-bridge',NULL,NULL,NULL,0,'Airport','CORR-DCS-90904',NULL,24),
    (@OrderId,25,'2026-09-12T12:25:00','ApplyDeliveryStatusChange','System','dcs-bridge',NULL,NULL,NULL,0,'Airport','CORR-DCS-91205',NULL,25),
    (@OrderId,26,'2026-09-12T12:30:00','ApplyDeliveryStatusChange','System','dcs-bridge',NULL,NULL,NULL,0,'Airport','CORR-DCS-91206',NULL,26);

COMMIT TRANSACTION;
PRINT '05-flown: all live units Flown, ancillaries Consumed, flight items Used';
