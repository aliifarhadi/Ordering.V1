using AeroTech.Framework.Core.Domain.Aggregates;
using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Enums;
using AeroTech.Ordering.Domain._Shared.Identifiers;
using AeroTech.Ordering.Domain._Shared.Resources;
using AeroTech.Ordering.Domain.OperationAggregate.Arguments;
using AeroTech.Ordering.Domain.OperationAggregate.DomainEvents;
using AeroTech.Ordering.Domain.OperationAggregate.Entities;
using NodaTime;
using IClock = AeroTech.Framework.Core.ServiceContracts.IClock;
using IIdGenerator = AeroTech.Framework.Core.ServiceContracts.IIdGenerator;

namespace AeroTech.Ordering.Domain.OperationAggregate
{
    [SpecRef("DD-AGG-002")]
    public sealed class Operation : AggregateRoot<OperationId>
    {
        private readonly List<OperationStep> _stepHistory = [];
        private readonly List<OrderItemId> _scopedItems = [];

        private Operation()
        {
        }

        private Operation(
            OperationId id,
            StartOperationArguments arguments,
            Instant startedAtUtc)
        {
            if (string.IsNullOrWhiteSpace(arguments.CorrelationId))
                throw ExceptionFactory.IdentifierIsRequired(nameof(arguments.CorrelationId));

            if (string.IsNullOrWhiteSpace(arguments.IdempotencyKey))
                throw ExceptionFactory.IdentifierIsRequired(nameof(arguments.IdempotencyKey));

            Id = id;
            Type = arguments.Type;
            OrderRef = arguments.OrderRef;
            GroupRef = arguments.GroupRef;
            Status = OperationStatus.Running;
            CurrentStep = arguments.CurrentStep;
            CorrelationId = arguments.CorrelationId;
            CausationId = arguments.CausationId;
            AttemptCount = 0;
            TimeoutAtUtc = arguments.TimeoutAtUtc;
            CompensationPolicy = arguments.CompensationPolicy;
            IdempotencyKey = arguments.IdempotencyKey;

            _scopedItems = [.. arguments.ScopedItems];
            _stepHistory.Add(new OperationStep(arguments.CurrentStep, startedAtUtc));
        }

        [SpecRef("DD-AGG-002-02")]
        public OperationType Type { get; private set; }

        [SpecRef("DD-AGG-002-03")]
        public OrderId? OrderRef { get; private set; }

        [SpecRef("CDR-4.1.1-GroupRef")]
        public GroupBookingId? GroupRef { get; private set; }

        [SpecRef("DD-AGG-002-04")]
        public OperationStatus Status { get; private set; }

        [SpecRef("DD-AGG-002-05")]
        public string CurrentStep { get; private set; } = null!;

        [SpecRef("DD-AGG-002-06")]
        public IReadOnlyList<OperationStep> StepHistory => _stepHistory.AsReadOnly();

        [SpecRef("DD-AGG-002-07")]
        public string? ExpectedExternalMessage { get; private set; }

        [SpecRef("DD-AGG-002-08")]
        public string CorrelationId { get; private set; } = null!;

        [SpecRef("DD-AGG-002-09")]
        public string? CausationId { get; private set; }

        [SpecRef("DD-AGG-002-10")]
        public int AttemptCount { get; private set; }

        [SpecRef("DD-AGG-002-11")]
        public Instant? NextRetryAtUtc { get; private set; }

        [SpecRef("DD-AGG-002-12")]
        public Instant? TimeoutAtUtc { get; private set; }

        [SpecRef("DD-AGG-002-13")]
        public CompensationPolicy CompensationPolicy { get; private set; }

        [SpecRef("DD-AGG-002-14")]
        public string? Result { get; private set; }

        [SpecRef("DD-AGG-002-15")]
        public string? FailureReason { get; private set; }

        [SpecRef("DD-AGG-002-16")]
        public string IdempotencyKey { get; private set; } = null!;

        [SpecRef("DD-AGG-002-17")]
        public IReadOnlyList<OrderItemId> ScopedItems => _scopedItems.AsReadOnly();

        [SpecRef("DD-AGG-002-04")]
        public bool IsOpen => Status is OperationStatus.Running or OperationStatus.AwaitingExternal;

        [SpecRef("DD-AGG-002")]
        public static Operation Start(
            StartOperationArguments arguments,
            IIdGenerator idGenerator,
            IClock clock)
        {
            var now = Instant.FromDateTimeOffset(clock.GetDateTime());

            var operation = new Operation(OperationId.New(), arguments, now);

            operation.Causes(new OperationStarted(
                EventId: idGenerator.NewId().ToString(),
                AggregateId: operation.Id.ToString(),
                Type: operation.Type,
                TimeOfOccurrence: clock.GetDateTime()));

            return operation;
        }

        [SpecRef("DD-AGG-002-05")]
        public void Advance(
            AdvanceOperationArguments arguments,
            IIdGenerator idGenerator,
            IClock clock)
        {
            EnsureNotTerminal();

            var now = Instant.FromDateTimeOffset(clock.GetDateTime());

            CompleteOpenStep(arguments.PreviousStepOutcome, now);

            CurrentStep = arguments.NextStep;
            Status = OperationStatus.Running;
            ExpectedExternalMessage = null;
            NextRetryAtUtc = null;
            _stepHistory.Add(new OperationStep(arguments.NextStep, now));

            Causes(new OperationStepAdvanced(
                EventId: idGenerator.NewId().ToString(),
                AggregateId: Id.ToString(),
                CurrentStep: CurrentStep,
                TimeOfOccurrence: clock.GetDateTime()));
        }

        [SpecRef("DD-AGG-002-07")]
        public void AwaitExternal(
            AwaitExternalArguments arguments,
            IIdGenerator idGenerator,
            IClock clock)
        {
            EnsureNotTerminal();

            if (string.IsNullOrWhiteSpace(arguments.ExpectedExternalMessage))
                throw ExceptionFactory.IdentifierIsRequired(nameof(arguments.ExpectedExternalMessage));

            ExpectedExternalMessage = arguments.ExpectedExternalMessage;
            NextRetryAtUtc = arguments.NextRetryAtUtc;
            AttemptCount++;

            TransitionTo(OperationStatus.AwaitingExternal, idGenerator, clock);
        }

        [SpecRef("DD-AGG-002-14")]
        public void Complete(string? result, IIdGenerator idGenerator, IClock clock)
        {
            EnsureNotTerminal();

            var now = Instant.FromDateTimeOffset(clock.GetDateTime());
            CompleteOpenStep(result, now);

            Result = result;
            ExpectedExternalMessage = null;
            NextRetryAtUtc = null;

            TransitionTo(OperationStatus.Completed, idGenerator, clock);
        }

        [SpecRef("DD-AGG-002-15")]
        public void Fail(string failureReason, IIdGenerator idGenerator, IClock clock)
        {
            EnsureNotTerminal();

            var now = Instant.FromDateTimeOffset(clock.GetDateTime());
            CompleteOpenStep(failureReason, now);

            FailureReason = failureReason;

            TransitionTo(OperationStatus.Failed, idGenerator, clock);
        }

        [SpecRef("DD-AGG-002-04")]
        public void RouteToManualReview(string reason, IIdGenerator idGenerator, IClock clock)
        {
            EnsureNotTerminal();

            FailureReason = reason;

            TransitionTo(OperationStatus.AwaitingManualReview, idGenerator, clock);
        }

        [SpecRef("DD-ENUM-018")]
        public void MarkCompensated(IIdGenerator idGenerator, IClock clock)
        {
            if (CompensationPolicy != CompensationPolicy.Compensate)
                throw ExceptionFactory.OperationCannotTransition(Id, Status, OperationStatus.Compensated);

            TransitionTo(OperationStatus.Compensated, idGenerator, clock);
        }

        private void TransitionTo(OperationStatus target, IIdGenerator idGenerator, IClock clock)
        {
            if (!IsTransitionPermitted(Status, target))
                throw ExceptionFactory.OperationCannotTransition(Id, Status, target);

            Status = target;

            Causes(new OperationStatusChanged(
                EventId: idGenerator.NewId().ToString(),
                AggregateId: Id.ToString(),
                Status: Status,
                TimeOfOccurrence: clock.GetDateTime()));
        }

        private void CompleteOpenStep(string? outcome, Instant completedAtUtc)
        {
            var open = _stepHistory.LastOrDefault(step => step.IsOpen);
            open?.Complete(outcome, completedAtUtc);
        }

        private void EnsureNotTerminal()
        {
            if (!IsOpen && Status != OperationStatus.AwaitingManualReview)
                throw ExceptionFactory.OperationIsTerminal(Id, Status);
        }

        [SpecRef("DD-ENUM-017")]
        private static bool IsTransitionPermitted(OperationStatus from, OperationStatus to) =>
            (from, to) switch
            {
                (OperationStatus.Running, OperationStatus.AwaitingExternal) => true,
                (OperationStatus.Running, OperationStatus.Completed) => true,
                (OperationStatus.Running, OperationStatus.Failed) => true,
                (OperationStatus.Running, OperationStatus.AwaitingManualReview) => true,
                (OperationStatus.AwaitingExternal, OperationStatus.Running) => true,
                (OperationStatus.AwaitingExternal, OperationStatus.Completed) => true,
                (OperationStatus.AwaitingExternal, OperationStatus.Failed) => true,
                (OperationStatus.AwaitingExternal, OperationStatus.AwaitingManualReview) => true,
                (OperationStatus.AwaitingManualReview, OperationStatus.Running) => true,
                (OperationStatus.AwaitingManualReview, OperationStatus.Completed) => true,
                (OperationStatus.AwaitingManualReview, OperationStatus.Failed) => true,
                (OperationStatus.Failed, OperationStatus.Compensated) => true,
                _ => false,
            };
    }
}
