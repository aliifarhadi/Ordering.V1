using AeroTech.Framework.Core.Domain.Aggregates;
using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Enums;
using AeroTech.Ordering.Domain._Shared.Identifiers;
using AeroTech.Ordering.Domain._Shared.Resources;
using AeroTech.Ordering.Domain._Shared.ValueObjects;
using AeroTech.Ordering.Domain.TaxDocumentAggregate.Arguments;
using AeroTech.Ordering.Domain.TaxDocumentAggregate.DomainEvents;
using AeroTech.Ordering.Domain.TaxDocumentAggregate.Entities;
using NodaTime;
using IClock = AeroTech.Framework.Core.ServiceContracts.IClock;
using IIdGenerator = AeroTech.Framework.Core.ServiceContracts.IIdGenerator;

namespace AeroTech.Ordering.Domain.TaxDocumentAggregate
{
    [SpecRef("CDR-3.2.1")]
    public sealed class TaxDocument : AggregateRoot<DocumentNumber>
    {
        private readonly List<TaxLine> _lines = [];
        private readonly List<SubmissionAttempt> _submissionAttempts = [];

        private TaxDocument()
        {
        }

        private TaxDocument(
            DocumentNumber number,
            IssueTaxDocumentArguments arguments,
            Instant issuedAtUtc)
        {
            Id = number;
            Kind = arguments.Kind;
            OrderRef = arguments.OrderRef;
            DeliveryRecordRef = arguments.DeliveryRecordRef;
            ReversesDocument = arguments.ReversesDocument;
            BranchRef = arguments.BranchRef;
            SpecVersion = arguments.SpecVersion;
            BuyerTaxIdentity = arguments.BuyerTaxIdentity;
            PassengerIdentity = arguments.PassengerIdentity;
            IssuedAtUtc = issuedAtUtc;
            TotalAmount = arguments.TotalAmount;
            TotalTax = arguments.TotalTax;
            SubmissionStatus = arguments.InitialSubmissionStatus;
            AggregateVersion = 0;
        }

        [SpecRef("CDR-3.2.1")]
        public DocumentNumber Number => Id;

        [SpecRef("CDR-3.2.1")]
        public TaxDocumentKind Kind { get; private set; }

        [SpecRef("CDR-3.2.1")]
        public OrderId OrderRef { get; private set; }

        [SpecRef("CDR-3.2.1")]
        public DeliveryRecordNumber? DeliveryRecordRef { get; private set; }

        [SpecRef("I-32")]
        public DocumentNumber? ReversesDocument { get; private set; }

        [SpecRef("CDR-3.2.1")]
        public BranchId BranchRef { get; private set; }

        [SpecRef("I-31")]
        public string SpecVersion { get; private set; } = null!;

        [SpecRef("CDR-3.2.1")]
        public string? BuyerTaxIdentity { get; private set; }

        [SpecRef("CDR-3.2.1")]
        public string PassengerIdentity { get; private set; } = null!;

        [SpecRef("CDR-3.2.1")]
        public Instant IssuedAtUtc { get; private set; }

        [SpecRef("CDR-3.2.1")]
        public Money TotalAmount { get; private set; } = null!;

        [SpecRef("CDR-3.2.1")]
        public Money TotalTax { get; private set; } = null!;

        [SpecRef("CDR-3.2.1")]
        public SubmissionStatus SubmissionStatus { get; private set; }

        [SpecRef("CDR-3.2.1")]
        public long AggregateVersion { get; private set; }

        [SpecRef("CDR-3.2.1")]
        public IReadOnlyList<TaxLine> Lines => _lines.AsReadOnly();

        [SpecRef("CDR-3.2.1")]
        public IReadOnlyList<SubmissionAttempt> SubmissionAttempts => _submissionAttempts.AsReadOnly();

        [SpecRef("CDR-3.2.4-Issue")]
        public static TaxDocument Issue(
            DocumentNumber number,
            IssueTaxDocumentArguments arguments,
            IIdGenerator idGenerator,
            IClock clock)
        {
            if (string.IsNullOrWhiteSpace(arguments.SpecVersion))
                throw ExceptionFactory.SpecVersionIsRequired();

            if (string.IsNullOrWhiteSpace(arguments.PassengerIdentity))
                throw ExceptionFactory.PassengerIdentityIsRequired();

            if (arguments.Lines.Count == 0)
                throw ExceptionFactory.TaxDocumentRequiresAtLeastOneLine();

            EnsureReversalReference(arguments);
            EnsureTotalsMatchLines(arguments);

            var now = Instant.FromDateTimeOffset(clock.GetDateTime());
            var document = new TaxDocument(number, arguments, now);

            var seq = 1;
            foreach (var line in arguments.Lines)
            {
                document._lines.Add(new TaxLine(
                    seq: seq++,
                    lineType: line.LineType,
                    code: line.Code,
                    description: line.Description,
                    netAmount: line.NetAmount,
                    taxAmount: line.TaxAmount,
                    taxRate: line.TaxRate,
                    taxJurisdiction: line.TaxJurisdiction));
            }

            document.AggregateVersion++;

            document.Causes(new TaxDocumentIssued(
                EventId: idGenerator.NewId().ToString(),
                AggregateId: document.Id.ToString(),
                Kind: document.Kind,
                OrderRef: document.OrderRef,
                AggregateVersion: document.AggregateVersion,
                TimeOfOccurrence: clock.GetDateTime()));

            return document;
        }

        [SpecRef("CDR-3.2.4-MarkSubmitted")]
        public void MarkSubmitted(
            MarkSubmittedArguments arguments,
            IIdGenerator idGenerator,
            IClock clock)
        {
            TransitionTo(SubmissionStatus.Submitted);

            var now = Instant.FromDateTimeOffset(clock.GetDateTime());
            RecordAttempt(now, nameof(SubmissionStatus.Submitted), arguments.ResponseReference, null);

            AggregateVersion++;

            Causes(new TaxDocumentSubmitted(
                EventId: idGenerator.NewId().ToString(),
                AggregateId: Id.ToString(),
                AggregateVersion: AggregateVersion,
                TimeOfOccurrence: clock.GetDateTime()));
        }

        [SpecRef("CDR-3.2.4-MarkAccepted")]
        public void MarkAccepted(
            MarkAcceptedArguments arguments,
            IIdGenerator idGenerator,
            IClock clock)
        {
            TransitionTo(SubmissionStatus.Accepted);

            var now = Instant.FromDateTimeOffset(clock.GetDateTime());
            RecordAttempt(now, nameof(SubmissionStatus.Accepted), arguments.ResponseReference, null);

            AggregateVersion++;

            Causes(new TaxDocumentAccepted(
                EventId: idGenerator.NewId().ToString(),
                AggregateId: Id.ToString(),
                ResponseReference: arguments.ResponseReference,
                AggregateVersion: AggregateVersion,
                TimeOfOccurrence: clock.GetDateTime()));
        }

        [SpecRef("CDR-3.2.4-MarkRejected")]
        public void MarkRejected(
            MarkRejectedArguments arguments,
            IIdGenerator idGenerator,
            IClock clock)
        {
            TransitionTo(SubmissionStatus.Rejected);

            var now = Instant.FromDateTimeOffset(clock.GetDateTime());
            RecordAttempt(
                now,
                nameof(SubmissionStatus.Rejected),
                arguments.ResponseReference,
                arguments.RejectionReason);

            AggregateVersion++;

            Causes(new TaxDocumentRejected(
                EventId: idGenerator.NewId().ToString(),
                AggregateId: Id.ToString(),
                RejectionReason: arguments.RejectionReason,
                AggregateVersion: AggregateVersion,
                TimeOfOccurrence: clock.GetDateTime()));
        }

        [SpecRef("CDR-3.2.4-Correct")]
        public void Correct(IIdGenerator idGenerator, IClock clock)
        {
            TransitionTo(SubmissionStatus.PendingSubmission);

            AggregateVersion++;

            Causes(new TaxDocumentCorrected(
                EventId: idGenerator.NewId().ToString(),
                AggregateId: Id.ToString(),
                AggregateVersion: AggregateVersion,
                TimeOfOccurrence: clock.GetDateTime()));
        }

        private void RecordAttempt(
            Instant attemptedAtUtc,
            string outcome,
            string? responseReference,
            string? rejectionReason) =>
            _submissionAttempts.Add(new SubmissionAttempt(
                seq: _submissionAttempts.Count + 1,
                attemptedAtUtc: attemptedAtUtc,
                outcome: outcome,
                responseReference: responseReference,
                rejectionReason: rejectionReason));

        private void TransitionTo(SubmissionStatus target)
        {
            if (!IsTransitionPermitted(SubmissionStatus, target))
                throw ExceptionFactory.SubmissionCannotTransition(Id, SubmissionStatus, target);

            SubmissionStatus = target;
        }

        [SpecRef("I-32")]
        private static void EnsureReversalReference(IssueTaxDocumentArguments arguments)
        {
            var isReversal = arguments.Kind is TaxDocumentKind.CreditNote or TaxDocumentKind.Cancellation;

            if (isReversal && arguments.ReversesDocument is null)
                throw ExceptionFactory.ReversalRequiresReversedDocument(arguments.Kind);

            if (!isReversal && arguments.ReversesDocument is not null)
                throw ExceptionFactory.InvoiceCannotReferenceReversedDocument();
        }

        private static void EnsureTotalsMatchLines(IssueTaxDocumentArguments arguments)
        {
            var currency = arguments.TotalAmount.Currency;

            var net = arguments.Lines.Aggregate(
                Money.Zero(currency),
                (running, line) => running + line.NetAmount);

            var tax = arguments.Lines.Aggregate(
                Money.Zero(currency),
                (running, line) => running + line.TaxAmount);

            if (tax != arguments.TotalTax || net + tax != arguments.TotalAmount)
                throw ExceptionFactory.TaxDocumentTotalsMustMatchLines();
        }

        [SpecRef("CDR-3.2.5")]
        private static bool IsTransitionPermitted(SubmissionStatus from, SubmissionStatus to) =>
            (from, to) switch
            {
                (SubmissionStatus.PendingSubmission, SubmissionStatus.Submitted) => true,
                (SubmissionStatus.Submitted, SubmissionStatus.Accepted) => true,
                (SubmissionStatus.Submitted, SubmissionStatus.Rejected) => true,
                (SubmissionStatus.Rejected, SubmissionStatus.PendingSubmission) => true,
                _ => false,
            };
    }
}
