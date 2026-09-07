using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Enums;
using AeroTech.Ordering.Domain._Shared.Identifiers;
using AeroTech.Ordering.Domain._Shared.Resources;

namespace AeroTech.Ordering.Domain._Shared.ValueObjects
{
    [SpecRef("DD-VO-017")]
    public sealed record ActorRef
    {
#pragma warning disable CS8618
        private ActorRef()
        {
        }
#pragma warning restore CS8618

        [SpecRef("DD-VO-017")]
        public ActorRef(
            ActorType actorType,
            string userId,
            SellerId? sellerId,
            BranchId? branchId,
            string? officeId,
            bool hasAirlineOverride)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw ExceptionFactory.IdentifierIsRequired(nameof(userId));

            ActorType = actorType;
            UserId = userId;
            SellerId = sellerId;
            BranchId = branchId;
            OfficeId = officeId;
            HasAirlineOverride = hasAirlineOverride;
        }

        [SpecRef("DD-VO-017")]
        public ActorType ActorType { get; }

        [SpecRef("DD-VO-017")]
        public string UserId { get; }

        [SpecRef("DD-VO-017")]
        public SellerId? SellerId { get; }

        [SpecRef("CDR-1.4-ActorRef")]
        public BranchId? BranchId { get; }

        [SpecRef("DD-VO-017")]
        public string? OfficeId { get; }

        [SpecRef("DD-VO-017")]
        public bool HasAirlineOverride { get; }
    }
}
