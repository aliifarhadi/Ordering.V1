using AeroTech.Framework.Core.Domain.Aggregates;
using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Enums;
using AeroTech.Ordering.Domain._Shared.Identifiers;
using AeroTech.Ordering.Domain._Shared.Resources;
using AeroTech.Ordering.Domain.DocumentNumberRangeAggregate.Arguments;
using NodaTime;

namespace AeroTech.Ordering.Domain.DocumentNumberRangeAggregate
{
    [SpecRef("CDR-3.3.1")]
    public sealed class DocumentNumberRange : AggregateRoot<RangeId>
    {
        private DocumentNumberRange()
        {
        }

        [SpecRef("CDR-3.3.1")]
        public DocumentNumberRange(RegisterRangeArguments arguments)
        {
            if (arguments.RangeStart > arguments.RangeEnd)
                throw ExceptionFactory.RangeBoundsAreInvalid();

            Id = RangeId.New();
            BranchRef = arguments.BranchRef;
            DocumentKind = arguments.DocumentKind;
            Prefix = arguments.Prefix;
            RangeStart = arguments.RangeStart;
            RangeEnd = arguments.RangeEnd;
            NextAvailable = arguments.RangeStart;
            Status = RangeStatus.Active;
            RegisteredWithAuthorityAtUtc = arguments.RegisteredWithAuthorityAtUtc;
            AuthorityReference = arguments.AuthorityReference;
        }

        [SpecRef("CDR-3.3.1")]
        public BranchId BranchRef { get; private set; }

        [SpecRef("CDR-3.3.1")]
        public TaxDocumentKind DocumentKind { get; private set; }

        [SpecRef("CDR-3.3.1")]
        public string? Prefix { get; private set; }

        [SpecRef("CDR-3.3.1")]
        public long RangeStart { get; private set; }

        [SpecRef("CDR-3.3.1")]
        public long RangeEnd { get; private set; }

        [SpecRef("CDR-3.3.1")]
        public long NextAvailable { get; private set; }

        [SpecRef("CDR-3.3.1")]
        public RangeStatus Status { get; private set; }

        [SpecRef("CDR-3.3.1")]
        public Instant? RegisteredWithAuthorityAtUtc { get; private set; }

        [SpecRef("CDR-3.3.1")]
        public string? AuthorityReference { get; private set; }

        [SpecRef("CDR-3.3.1")]
        public bool IsExhausted => NextAvailable > RangeEnd;

        [SpecRef("I-30")]
        public DocumentNumber AllocateNext(TaxDocumentKind documentKind)
        {
            if (Status != RangeStatus.Active)
                throw ExceptionFactory.RangeIsNotActive(Id);

            if (documentKind != DocumentKind)
                throw ExceptionFactory.RangeKindMismatch(Id, documentKind);

            if (IsExhausted)
                throw ExceptionFactory.RangeIsExhausted(Id);

            var allocated = NextAvailable;
            NextAvailable++;

            if (IsExhausted)
                Status = RangeStatus.Exhausted;

            return new DocumentNumber($"{Prefix}{allocated}");
        }

        [SpecRef("CDR-3.3.1")]
        public void Close() => Status = RangeStatus.Closed;

        [SpecRef("CDR-3.3.1")]
        public void RegisterWithAuthority(Instant registeredAtUtc, string authorityReference)
        {
            if (string.IsNullOrWhiteSpace(authorityReference))
                throw ExceptionFactory.IdentifierIsRequired(nameof(authorityReference));

            RegisteredWithAuthorityAtUtc = registeredAtUtc;
            AuthorityReference = authorityReference;
        }
    }
}
