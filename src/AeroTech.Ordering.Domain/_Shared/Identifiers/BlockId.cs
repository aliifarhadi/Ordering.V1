using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Resources;

namespace AeroTech.Ordering.Domain._Shared.Identifiers
{
    [SpecRef("CDR-ID-BlockId")]
    public readonly record struct BlockId
    {
        [SpecRef("CDR-ID-BlockId")]
        public BlockId(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw ExceptionFactory.IdentifierIsRequired(nameof(BlockId));

            Value = value;
        }

        [SpecRef("CDR-ID-BlockId")]
        public string Value { get; }

        [SpecRef("CDR-ID-BlockId")]
        public override string ToString() => Value;
    }
}
