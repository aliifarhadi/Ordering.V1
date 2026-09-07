using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Resources;

namespace AeroTech.Ordering.Domain._Shared.Identifiers
{
    [SpecRef("DD-ID-011")]
    public readonly record struct MarriedGroupId
    {
        [SpecRef("DD-ID-011")]
        public MarriedGroupId(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw ExceptionFactory.IdentifierIsRequired(nameof(MarriedGroupId));

            Value = value;
        }

        [SpecRef("DD-ID-011")]
        public string Value { get; }

        [SpecRef("DD-ID-011")]
        public override string ToString() => Value;
    }
}
