using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Resources;

namespace AeroTech.Ordering.Domain._Shared.Identifiers
{
    [SpecRef("CDR-ID-DeliveryRecordNumber")]
    public readonly record struct DeliveryRecordNumber
    {
        [SpecRef("CDR-ID-DeliveryRecordNumber")]
        public DeliveryRecordNumber(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw ExceptionFactory.IdentifierIsRequired(nameof(DeliveryRecordNumber));

            Value = value;
        }

        [SpecRef("CDR-ID-DeliveryRecordNumber")]
        public string Value { get; }

        [SpecRef("CDR-ID-DeliveryRecordNumber")]
        public override string ToString() => Value;
    }
}
