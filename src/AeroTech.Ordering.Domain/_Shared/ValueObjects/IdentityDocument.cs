using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Enums;
using AeroTech.Ordering.Domain._Shared.Resources;
using NodaTime;

namespace AeroTech.Ordering.Domain._Shared.ValueObjects
{
    [SpecRef("DD-VO-014")]
    public sealed record IdentityDocument
    {
#pragma warning disable CS8618
        private IdentityDocument()
        {
        }
#pragma warning restore CS8618

        [SpecRef("DD-VO-014")]
        [Blocked("BL-002", "Per-type checksum and format validation of the document number", "O13-B1")]
        public IdentityDocument(
            IdentityDocumentType docType,
            string number,
            string? issuingCountry,
            LocalDate? expiry,
            string? nationality)
        {
            if (string.IsNullOrWhiteSpace(number))
                throw ExceptionFactory.IdentifierIsRequired(nameof(number));

            DocType = docType;
            Number = number;
            IssuingCountry = issuingCountry;
            Expiry = expiry;
            Nationality = nationality;
        }

        [SpecRef("DD-VO-014")]
        public IdentityDocumentType DocType { get; }

        [SpecRef("DD-VO-014")]
        [Blocked("BL-002", "Document number content validation", "O13-B1")]
        public string Number { get; }

        [SpecRef("DD-VO-014")]
        public string? IssuingCountry { get; }

        [SpecRef("DD-VO-014")]
        public LocalDate? Expiry { get; }

        [SpecRef("DD-VO-014")]
        public string? Nationality { get; }
    }
}
