using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Enums;
using AeroTech.Ordering.Domain._Shared.Identifiers;
using NodaTime;

namespace AeroTech.Ordering.Domain.DocumentNumberRangeAggregate.Arguments
{
    [SpecRef("CDR-3.3.1")]
    public sealed record RegisterRangeArguments
    {
        [SpecRef("CDR-3.3.1")]
        public required BranchId BranchRef { get; init; }

        [SpecRef("CDR-3.3.1")]
        public required TaxDocumentKind DocumentKind { get; init; }

        [SpecRef("CDR-3.3.1")]
        public string? Prefix { get; init; }

        [SpecRef("CDR-3.3.1")]
        public required long RangeStart { get; init; }

        [SpecRef("CDR-3.3.1")]
        public required long RangeEnd { get; init; }

        [SpecRef("CDR-3.3.1")]
        public Instant? RegisteredWithAuthorityAtUtc { get; init; }

        [SpecRef("CDR-3.3.1")]
        public string? AuthorityReference { get; init; }
    }
}
