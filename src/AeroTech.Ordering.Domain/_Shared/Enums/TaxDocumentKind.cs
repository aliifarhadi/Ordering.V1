using AeroTech.Ordering.Domain._Shared.Attributes;

namespace AeroTech.Ordering.Domain._Shared.Enums
{
    [SpecRef("CDR-ENUM-TaxDocumentKind")]
    public enum TaxDocumentKind
    {
        [SpecRef("CDR-ENUM-TaxDocumentKind")]
        Invoice = 1,

        [SpecRef("CDR-ENUM-TaxDocumentKind")]
        CreditNote = 2,

        [SpecRef("CDR-ENUM-TaxDocumentKind")]
        Cancellation = 3,
    }
}
