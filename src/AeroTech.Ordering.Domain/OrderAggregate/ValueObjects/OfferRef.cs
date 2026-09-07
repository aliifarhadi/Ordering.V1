using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Identifiers;
using AeroTech.Ordering.Domain._Shared.Resources;
using NodaTime;

namespace AeroTech.Ordering.Domain.OrderAggregate.ValueObjects
{
    [SpecRef("DD-VO-019")]
    public sealed record OfferRef
    {
#pragma warning disable CS8618
        private OfferRef()
        {
        }
#pragma warning restore CS8618

        [SpecRef("DD-VO-019")]
        public OfferRef(OfferId offerId, string offerItemId, Instant offerExpiryUtc, string offerOwner)
        {
            if (string.IsNullOrWhiteSpace(offerItemId))
                throw ExceptionFactory.IdentifierIsRequired(nameof(offerItemId));

            if (string.IsNullOrWhiteSpace(offerOwner))
                throw ExceptionFactory.IdentifierIsRequired(nameof(offerOwner));

            OfferId = offerId;
            OfferItemId = offerItemId;
            OfferExpiryUtc = offerExpiryUtc;
            OfferOwner = offerOwner;
        }

        [SpecRef("DD-VO-019")]
        public OfferId OfferId { get; }

        [SpecRef("DD-VO-019")]
        public string OfferItemId { get; }

        [SpecRef("DD-VO-019")]
        public Instant OfferExpiryUtc { get; }

        [SpecRef("DD-VO-019")]
        public string OfferOwner { get; }
    }
}
