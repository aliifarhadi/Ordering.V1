using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Enums;
using AeroTech.Ordering.Domain._Shared.Resources;

namespace AeroTech.Ordering.Domain._Shared.ValueObjects
{
    [SpecRef("DD-VO-015")]
    public sealed record ContactPoint
    {
#pragma warning disable CS8618
        private ContactPoint()
        {
        }
#pragma warning restore CS8618

        [SpecRef("DD-VO-015")]
        public ContactPoint(ContactType type, string value, bool isPrimary)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw ExceptionFactory.ContactValueIsRequired();

            Type = type;
            Value = value;
            IsPrimary = isPrimary;
        }

        [SpecRef("DD-VO-015")]
        public ContactType Type { get; }

        [SpecRef("DD-VO-015")]
        public string Value { get; }

        [SpecRef("DD-VO-015")]
        public bool IsPrimary { get; }
    }
}
