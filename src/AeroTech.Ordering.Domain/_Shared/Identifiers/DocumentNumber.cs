using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Resources;

namespace AeroTech.Ordering.Domain._Shared.Identifiers
{
    [SpecRef("CDR-ID-DocumentNumber")]
    public readonly record struct DocumentNumber
    {
        [SpecRef("CDR-ID-DocumentNumber")]
        public DocumentNumber(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw ExceptionFactory.IdentifierIsRequired(nameof(DocumentNumber));

            Value = value;
        }

        [SpecRef("CDR-ID-DocumentNumber")]
        public string Value { get; }

        [SpecRef("CDR-ID-DocumentNumber")]
        public override string ToString() => Value;
    }
}
