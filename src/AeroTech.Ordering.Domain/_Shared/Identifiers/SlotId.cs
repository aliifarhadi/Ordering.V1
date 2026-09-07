using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Resources;

namespace AeroTech.Ordering.Domain._Shared.Identifiers
{
    [SpecRef("CDR-ID-SlotId")]
    public readonly record struct SlotId
    {
        [SpecRef("CDR-ID-SlotId")]
        public SlotId(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw ExceptionFactory.IdentifierIsRequired(nameof(SlotId));

            Value = value;
        }

        [SpecRef("CDR-ID-SlotId")]
        public string Value { get; }

        [SpecRef("CDR-ID-SlotId")]
        public override string ToString() => Value;
    }
}
