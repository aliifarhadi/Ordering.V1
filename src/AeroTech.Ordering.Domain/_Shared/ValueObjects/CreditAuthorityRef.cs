using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Resources;
using NodaTime;

namespace AeroTech.Ordering.Domain._Shared.ValueObjects
{
    [SpecRef("DD-VO-025")]
    public sealed record CreditAuthorityRef
    {
#pragma warning disable CS8618
        private CreditAuthorityRef()
        {
        }
#pragma warning restore CS8618

        [SpecRef("DD-VO-025")]
        public CreditAuthorityRef(string value, Instant grantedAtUtc)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw ExceptionFactory.IdentifierIsRequired(nameof(CreditAuthorityRef));

            Value = value;
            GrantedAtUtc = grantedAtUtc;
        }

        [SpecRef("DD-VO-025")]
        public string Value { get; }

        [SpecRef("DD-VO-025")]
        public Instant GrantedAtUtc { get; }
    }
}
