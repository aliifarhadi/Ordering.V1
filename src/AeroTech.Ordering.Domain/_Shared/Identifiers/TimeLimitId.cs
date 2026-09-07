using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Resources;

namespace AeroTech.Ordering.Domain._Shared.Identifiers
{
    [SpecRef("DD-ID-006")]
    public readonly record struct TimeLimitId
    {
        [SpecRef("DD-ID-006")]
        public TimeLimitId(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw ExceptionFactory.IdentifierIsRequired(nameof(TimeLimitId));

            Value = value;
        }

        [SpecRef("DD-ID-006")]
        public string Value { get; }

        [SpecRef("DD-ID-006")]
        public override string ToString() => Value;
    }
}
