using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Resources;

namespace AeroTech.Ordering.Domain._Shared.Identifiers
{
    [SpecRef("CDR-ID-BranchId")]
    public readonly record struct BranchId
    {
        [SpecRef("CDR-ID-BranchId")]
        public BranchId(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw ExceptionFactory.IdentifierIsRequired(nameof(BranchId));

            Value = value;
        }

        [SpecRef("CDR-ID-BranchId")]
        public string Value { get; }

        [SpecRef("CDR-ID-BranchId")]
        public override string ToString() => Value;
    }
}
