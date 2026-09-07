/*  00-reset.sql
    Removes the sample order K7QM24 and everything derived from it.
    Safe to run repeatedly; safe to run before any other seed file.
    Run order: 00 -> 01 -> 02 -> 03 -> 04 -> 05
*/
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;
BEGIN TRANSACTION;

DECLARE @OrderId uniqueidentifier = '0195f3a0-1111-7000-8000-000000000001';

DELETE FROM Delivery.TaxDocumentSubmissionAttempts
WHERE DocumentNumber IN (SELECT DocumentNumber FROM Delivery.TaxDocuments WHERE OrderId = @OrderId);

DELETE FROM Delivery.TaxDocumentLines
WHERE DocumentNumber IN (SELECT DocumentNumber FROM Delivery.TaxDocuments WHERE OrderId = @OrderId);

DELETE FROM Delivery.TaxDocuments WHERE OrderId = @OrderId;

DELETE FROM Delivery.SegmentDeliveryStatusHistory
WHERE DeliveryRecordNumber IN (SELECT DeliveryRecordNumber FROM Delivery.DeliveryRecords WHERE OrderId = @OrderId);

DELETE FROM Delivery.ServiceDeliveries
WHERE DeliveryRecordNumber IN (SELECT DeliveryRecordNumber FROM Delivery.DeliveryRecords WHERE OrderId = @OrderId);

DELETE FROM Delivery.SegmentDeliveries
WHERE DeliveryRecordNumber IN (SELECT DeliveryRecordNumber FROM Delivery.DeliveryRecords WHERE OrderId = @OrderId);

DELETE FROM Delivery.DeliveryRecords WHERE OrderId = @OrderId;

DELETE FROM Delivery.DocumentNumberRanges WHERE RangeId = '0195f3a0-2222-7000-8000-000000000001';

DELETE FROM Ops.OperationSteps
WHERE OperationId IN (SELECT Id FROM Ops.Operations WHERE OrderId = @OrderId);

DELETE FROM Ops.Operations WHERE OrderId = @OrderId;

DELETE FROM Audit.OrderHistory WHERE OrderId = @OrderId;

DELETE FROM Ordering.PaymentAllocations WHERE OrderId = @OrderId;
DELETE FROM Ordering.PaymentRecords    WHERE OrderId = @OrderId;
DELETE FROM Ordering.TimeLimits        WHERE OrderId = @OrderId;
DELETE FROM Ordering.ValueAllocations  WHERE OrderId = @OrderId;
DELETE FROM Ordering.ChargeLines       WHERE OrderId = @OrderId;
DELETE FROM Ordering.OrderItems        WHERE OrderId = @OrderId;
DELETE FROM Ordering.TravelerJourneySegments WHERE OrderId = @OrderId;
DELETE FROM Ordering.JourneyElements   WHERE OrderId = @OrderId;
DELETE FROM Ordering.TravelerDocuments WHERE OrderId = @OrderId;
DELETE FROM Ordering.Travelers         WHERE OrderId = @OrderId;
DELETE FROM Ordering.ContactPoints     WHERE OrderId = @OrderId;
DELETE FROM Ordering.Orders            WHERE Id = @OrderId;

COMMIT TRANSACTION;
PRINT '00-reset: sample order K7QM24 removed';
