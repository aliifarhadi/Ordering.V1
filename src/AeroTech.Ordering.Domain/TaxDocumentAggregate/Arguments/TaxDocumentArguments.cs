using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Enums;
using AeroTech.Ordering.Domain._Shared.Identifiers;
using AeroTech.Ordering.Domain._Shared.ValueObjects;

namespace AeroTech.Ordering.Domain.TaxDocumentAggregate.Arguments
{
    [SpecRef("CDR-3.2.4-Issue")]
    public sealed record IssueTaxDocumentArguments
    {
        [SpecRef("CDR-3.2.1")]
        public required TaxDocumentKind Kind { get; init; }

        [SpecRef("CDR-3.2.1")]
        public required OrderId OrderRef { get; init; }

        [SpecRef("CDR-3.2.1")]
        public DeliveryRecordNumber? DeliveryRecordRef { get; init; }

        [SpecRef("I-32")]
        public DocumentNumber? ReversesDocument { get; init; }

        [SpecRef("CDR-3.2.1")]
        public required BranchId BranchRef { get; init; }

        [SpecRef("I-31")]
        public required string SpecVersion { get; init; }

        [SpecRef("CDR-3.2.1")]
        public string? BuyerTaxIdentity { get; init; }

        [SpecRef("CDR-3.2.1")]
        public required string PassengerIdentity { get; init; }

        [SpecRef("CDR-3.2.1")]
        public required Money TotalAmount { get; init; }

        [SpecRef("CDR-3.2.1")]
        public required Money TotalTax { get; init; }

        [SpecRef("CDR-3.2.1")]
        public required SubmissionStatus InitialSubmissionStatus { get; init; }

        [SpecRef("CDR-3.2.2")]
        public required IReadOnlyList<IssuedTaxLine> Lines { get; init; }
    }

    [SpecRef("CDR-3.2.2")]
    public sealed record IssuedTaxLine
    {
        [SpecRef("CDR-3.2.2")]
        public required string LineType { get; init; }

        [SpecRef("CDR-3.2.2")]
        public required string Code { get; init; }

        [SpecRef("CDR-3.2.2")]
        public required string Description { get; init; }

        [SpecRef("CDR-3.2.2")]
        public required Money NetAmount { get; init; }

        [SpecRef("CDR-3.2.2")]
        public required Money TaxAmount { get; init; }

        [SpecRef("CDR-3.2.2")]
        public decimal? TaxRate { get; init; }

        [SpecRef("CDR-3.2.2")]
        public string? TaxJurisdiction { get; init; }
    }

    [SpecRef("CDR-3.2.4-MarkSubmitted")]
    public sealed record MarkSubmittedArguments
    {
        [SpecRef("CDR-3.2.3")]
        public string? ResponseReference { get; init; }
    }

    [SpecRef("CDR-3.2.4-MarkAccepted")]
    public sealed record MarkAcceptedArguments
    {
        [SpecRef("CDR-3.2.3")]
        public required string ResponseReference { get; init; }
    }

    [SpecRef("CDR-3.2.4-MarkRejected")]
    public sealed record MarkRejectedArguments
    {
        [SpecRef("CDR-3.2.3")]
        public required string RejectionReason { get; init; }

        [SpecRef("CDR-3.2.3")]
        public string? ResponseReference { get; init; }
    }
}
