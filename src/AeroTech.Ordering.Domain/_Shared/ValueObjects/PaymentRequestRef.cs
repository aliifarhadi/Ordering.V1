using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Resources;

namespace AeroTech.Ordering.Domain._Shared.ValueObjects
{
    [SpecRef("DD-VO-024")]
    public readonly record struct PaymentRequestRef
    {
        [SpecRef("DD-VO-024")]
        public PaymentRequestRef(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw ExceptionFactory.IdentifierIsRequired(nameof(PaymentRequestRef));

            Value = value;
        }

        [SpecRef("DD-VO-024")]
        public string Value { get; }

        [SpecRef("DD-VO-024")]
        public override string ToString() => Value;
    }
}
