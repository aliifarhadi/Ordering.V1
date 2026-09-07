using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Resources;

namespace AeroTech.Ordering.Domain._Shared.ValueObjects
{
    [SpecRef("DD-VO-012")]
    public readonly record struct RbdCode
    {
        [SpecRef("DD-VO-012")]
        public RbdCode(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw ExceptionFactory.IdentifierIsRequired(nameof(RbdCode));

            Value = value;
        }

        [SpecRef("DD-VO-012")]
        public string Value { get; }

        [SpecRef("DD-VO-012")]
        public override string ToString() => Value;
    }
}
