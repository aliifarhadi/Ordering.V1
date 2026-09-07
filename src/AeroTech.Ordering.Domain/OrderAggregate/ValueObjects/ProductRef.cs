using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Enums;
using AeroTech.Ordering.Domain._Shared.Resources;

namespace AeroTech.Ordering.Domain.OrderAggregate.ValueObjects
{
    [SpecRef("DD-VO-018")]
    [Blocked("BL-006", "Fare basis, brand, and fare family", "Pricing contract")]
    public sealed record ProductRef
    {
#pragma warning disable CS8618
        private ProductRef()
        {
        }
#pragma warning restore CS8618

        [SpecRef("DD-VO-018")]
        public ProductRef(string productCode, ItemType productType, string? sourceSystemRef)
        {
            if (string.IsNullOrWhiteSpace(productCode))
                throw ExceptionFactory.IdentifierIsRequired(nameof(productCode));

            ProductCode = productCode;
            ProductType = productType;
            SourceSystemRef = sourceSystemRef;
        }

        [SpecRef("DD-VO-018-01")]
        public string ProductCode { get; }

        [SpecRef("DD-VO-018-02")]
        public ItemType ProductType { get; }

        [SpecRef("DD-VO-018-03")]
        public string? SourceSystemRef { get; }
    }
}
