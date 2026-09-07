using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Resources;
using NodaTime;

namespace AeroTech.Ordering.Domain.TaxDocumentAggregate.Entities
{
    [SpecRef("CDR-3.2.3")]
    public sealed class SubmissionAttempt
    {
        private SubmissionAttempt()
        {
        }

        [SpecRef("CDR-3.2.3")]
        internal SubmissionAttempt(
            int seq,
            Instant attemptedAtUtc,
            string outcome,
            string? responseReference,
            string? rejectionReason)
        {
            if (string.IsNullOrWhiteSpace(outcome))
                throw ExceptionFactory.IdentifierIsRequired(nameof(outcome));

            Seq = seq;
            AttemptedAtUtc = attemptedAtUtc;
            Outcome = outcome;
            ResponseReference = responseReference;
            RejectionReason = rejectionReason;
        }

        [SpecRef("CDR-3.2.3")]
        public int Seq { get; private set; }

        [SpecRef("CDR-3.2.3")]
        public Instant AttemptedAtUtc { get; private set; }

        [SpecRef("CDR-3.2.3")]
        public string Outcome { get; private set; } = null!;

        [SpecRef("CDR-3.2.3")]
        public string? ResponseReference { get; private set; }

        [SpecRef("CDR-3.2.3")]
        public string? RejectionReason { get; private set; }
    }
}
