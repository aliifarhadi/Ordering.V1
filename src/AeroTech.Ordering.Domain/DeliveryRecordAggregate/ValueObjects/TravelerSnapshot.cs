using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Enums;
using AeroTech.Ordering.Domain._Shared.ValueObjects;
using NodaTime;

namespace AeroTech.Ordering.Domain.DeliveryRecordAggregate.ValueObjects
{
    [SpecRef("CDR-3.4-TravelerSnapshot")]
    public sealed record TravelerSnapshot
    {
#pragma warning disable CS8618
        private TravelerSnapshot()
        {
        }
#pragma warning restore CS8618

        [SpecRef("CDR-3.4-TravelerSnapshot")]
        public TravelerSnapshot(
            TravelerName name,
            PassengerTypeCode ptc,
            LocalDate? dateOfBirth,
            IdentityDocument? primaryDocument)
        {
            Name = name;
            Ptc = ptc;
            DateOfBirth = dateOfBirth;
            PrimaryDocument = primaryDocument;
        }

        [SpecRef("CDR-3.4-TravelerSnapshot")]
        public TravelerName Name { get; }

        [SpecRef("CDR-3.4-TravelerSnapshot")]
        public PassengerTypeCode Ptc { get; }

        [SpecRef("CDR-3.4-TravelerSnapshot")]
        public LocalDate? DateOfBirth { get; }

        [SpecRef("CDR-3.4-TravelerSnapshot")]
        public IdentityDocument? PrimaryDocument { get; }
    }
}
