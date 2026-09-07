/*  02-payment-settled.sql
    Card payment settled and allocated across all four items.
    Items advance PendingPayment -> Confirmed; the payment time limit is Met.

    Total 61,300,000 IRR = 28,000,000 + 28,000,000 + 3,500,000 + 1,800,000

    Depends on: 01-order-created.sql
*/
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
SET NOCOUNT ON;
SET XACT_ABORT ON;
BEGIN TRANSACTION;

DECLARE @OrderId uniqueidentifier = '0195f3a0-1111-7000-8000-000000000001';

INSERT INTO Ordering.PaymentRecords
    (OrderId, PaymentRecordId, FormOfPayment, Amount, Currency, Status, PaymentRequestRef,
     FxFrom, FxTo, FxRate, FxSource, FxCapturedAtUtc, PayerRef, RecordedAtUtc, IsUnderDispute)
VALUES
    (@OrderId, 'PR000001', 'Card', 61300000.0000, 'IRR', 'Settled', 'PAYREQ-2026-0807-77120',
     NULL, NULL, NULL, NULL, NULL, 'PAYER-ALI-REZAEI', '2026-08-07T09:12:00', 0);

INSERT INTO Ordering.PaymentAllocations
    (OrderId, PaymentRecordId, OrderItemId, Amount, Currency)
VALUES
    (@OrderId, 'PR000001', 'OI000001', 28000000.0000, 'IRR'),
    (@OrderId, 'PR000001', 'OI000002', 28000000.0000, 'IRR'),
    (@OrderId, 'PR000001', 'OI000003',  3500000.0000, 'IRR'),
    (@OrderId, 'PR000001', 'OI000004',  1800000.0000, 'IRR');

UPDATE Ordering.OrderItems
SET Status = 'Confirmed'
WHERE OrderId = @OrderId AND Status = 'PendingPayment';

UPDATE Ordering.TimeLimits
SET Status = 'Met'
WHERE OrderId = @OrderId AND TimeLimitId = 'TL000001';

UPDATE Ordering.Orders
SET AggregateVersion = 15, LastUpdateTime = '2026-08-07T09:12:00+00:00'
WHERE Id = @OrderId;

INSERT INTO Audit.OrderHistory
    (OrderId, SequenceNo, OccurredAtUtc, CommandName, ActorType, ActorUserId, ActorSellerId,
     ActorBranchId, ActorOfficeId, ActorHasAirlineOverride, ChannelCode, CorrelationId, CausationId,
     ResultingAggregateVersion)
VALUES
    (@OrderId,13,'2026-08-07T09:06:00','RecordPaymentRequest','System','worker-payment',NULL,NULL,NULL,0,'DirectIbe','CORR-PAY-77120','CORR-BOOK-77120',13),
    (@OrderId,14,'2026-08-07T09:11:00','ApplyPaymentResult','System','worker-payment',NULL,NULL,NULL,0,'DirectIbe','CORR-PAY-77120','CORR-BOOK-77120',14),
    (@OrderId,15,'2026-08-07T09:12:00','AllocatePayment','System','worker-payment',NULL,NULL,NULL,0,'DirectIbe','CORR-PAY-77120','CORR-BOOK-77120',15);

INSERT INTO Ops.Operations
    (Id, Type, Status, OrderId, GroupBookingId, CurrentStep, ExpectedExternalMessage,
     CorrelationId, CausationId, AttemptCount, NextRetryAtUtc, TimeoutAtUtc,
     CompensationPolicy, Result, FailureReason, IdempotencyKey, ScopedItems, LastUpdateTime, LastUpdatedBy)
VALUES
    ('0195f3a0-3333-7000-8000-000000000002','PaymentCompletion','Completed', @OrderId, NULL,'allocate',NULL,
     'CORR-PAY-77120','CORR-BOOK-77120',1,NULL,NULL,'ForwardOnly','settled',NULL,'payment:K7QM24',
     'OI000001,OI000002,OI000003,OI000004','2026-08-07T09:12:00+00:00',NULL);

SET IDENTITY_INSERT Ops.OperationSteps ON;
INSERT INTO Ops.OperationSteps (OperationId, Seq, Name, EnteredAtUtc, CompletedAtUtc, Outcome, Detail)
VALUES
    ('0195f3a0-3333-7000-8000-000000000002',5,'request-payment','2026-08-07T09:06:00','2026-08-07T09:07:00','ok','PAYREQ-2026-0807-77120'),
    ('0195f3a0-3333-7000-8000-000000000002',6,'await-result','2026-08-07T09:07:00','2026-08-07T09:11:00','settled',NULL),
    ('0195f3a0-3333-7000-8000-000000000002',7,'allocate','2026-08-07T09:11:00','2026-08-07T09:12:00','ok','4 allocations');
SET IDENTITY_INSERT Ops.OperationSteps OFF;

COMMIT TRANSACTION;
PRINT '02-payment-settled: 61,300,000 IRR settled, items Confirmed';
