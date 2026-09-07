using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Resources;
using NodaTime;

namespace AeroTech.Ordering.Domain.OperationAggregate.Entities
{
    [SpecRef("DD-VO-029")]
    public sealed class OperationStep
    {
        private OperationStep()
        {
        }

        [SpecRef("DD-VO-029")]
        internal OperationStep(string name, Instant enteredAtUtc)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw ExceptionFactory.IdentifierIsRequired(nameof(name));

            Name = name;
            EnteredAtUtc = enteredAtUtc;
        }

        [SpecRef("DD-VO-029")]
        public string Name { get; private set; } = null!;

        [SpecRef("DD-VO-029")]
        public Instant EnteredAtUtc { get; private set; }

        [SpecRef("DD-VO-029")]
        public Instant? CompletedAtUtc { get; private set; }

        [SpecRef("DD-VO-029")]
        public string? Outcome { get; private set; }

        [SpecRef("CDR-4.1.2-Detail")]
        public string? Detail { get; private set; }

        [SpecRef("DD-VO-029")]
        public bool IsOpen => CompletedAtUtc is null;

        [SpecRef("DD-VO-029")]
        internal void Complete(string? outcome, Instant completedAtUtc, string? detail = null)
        {
            CompletedAtUtc = completedAtUtc;
            Outcome = outcome;
            Detail = detail;
        }
    }
}
