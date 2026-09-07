using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Resources;

namespace AeroTech.Ordering.Domain._Shared.Identifiers
{
    [SpecRef("DD-ID-005")]
    public readonly record struct PaymentRecordId
    {
        [SpecRef("DD-ID-005")]
        public PaymentRecordId(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw ExceptionFactory.IdentifierIsRequired(nameof(PaymentRecordId));

            Value = value;
        }

        [SpecRef("DD-ID-005")]
        public string Value { get; }

        [SpecRef("DD-ID-005")]
        public override string ToString() => Value;
    }
}
