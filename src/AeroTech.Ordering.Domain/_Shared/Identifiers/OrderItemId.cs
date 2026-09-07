using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Resources;

namespace AeroTech.Ordering.Domain._Shared.Identifiers
{
    [SpecRef("DD-ID-004")]
    public readonly record struct OrderItemId
    {
        [SpecRef("DD-ID-004")]
        public OrderItemId(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw ExceptionFactory.IdentifierIsRequired(nameof(OrderItemId));

            Value = value;
        }

        [SpecRef("DD-ID-004")]
        public string Value { get; }

        [SpecRef("DD-ID-004")]
        public override string ToString() => Value;
    }
}
