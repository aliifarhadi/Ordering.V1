using AeroTech.Framework.Core.Domain.Entities;
using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Contracts;
using AeroTech.Ordering.Domain._Shared.Enums;
using AeroTech.Ordering.Domain._Shared.Identifiers;
using AeroTech.Ordering.Domain._Shared.Resources;
using AeroTech.Ordering.Domain._Shared.ValueObjects;
using NodaTime;

namespace AeroTech.Ordering.Domain.OrderAggregate.Entities
{
    [SpecRef("DD-ENT-001")]
    public sealed class Traveler : Entity<TravelerId>
    {
        private readonly List<IdentityDocument> _documents = [];

        private Traveler()
        {
        }

        [SpecRef("DD-ENT-001")]
        internal Traveler(
            TravelerId id,
            TravelerName name,
            PassengerTypeCode ptc,
            LocalDate? dateOfBirth,
            IReadOnlyList<IdentityDocument> documents,
            TravelerId? associatedAdult,
            CustomerId? customerRef)
        {
            if (ptc is PassengerTypeCode.CHD or PassengerTypeCode.INF && dateOfBirth is null)
                throw ExceptionFactory.DateOfBirthRequiredForPassengerType(ptc);

            if (associatedAdult is not null && associatedAdult.Value == id)
                throw ExceptionFactory.AssociatedAdultMustNotBeSelf(id);

            Id = id;
            Name = name;
            Ptc = ptc;
            DateOfBirth = dateOfBirth;
            AssociatedAdult = associatedAdult;
            CustomerRef = customerRef;
            _documents = [.. documents];
        }

        [SpecRef("DD-ENT-001-02")]
        public TravelerName Name { get; private set; } = null!;

        [SpecRef("DD-ENT-001-03")]
        public PassengerTypeCode Ptc { get; private set; }

        [SpecRef("DD-ENT-001-04")]
        public LocalDate? DateOfBirth { get; private set; }

        [SpecRef("DD-ENT-001-05")]
        public IReadOnlyList<IdentityDocument> Documents => _documents.AsReadOnly();

        [SpecRef("DD-ENT-001-06")]
        public TravelerId? AssociatedAdult { get; private set; }

        [SpecRef("DD-ENT-001-07")]
        public CustomerId? CustomerRef { get; private set; }

        [SpecRef("DD-CMD-003")]
        internal void UpdateDetails(
            TravelerName name,
            LocalDate? dateOfBirth,
            IReadOnlyList<IdentityDocument> documents,
            TravelerId? associatedAdult,
            CustomerId? customerRef)
        {
            if (Ptc is PassengerTypeCode.CHD or PassengerTypeCode.INF && dateOfBirth is null)
                throw ExceptionFactory.DateOfBirthRequiredForPassengerType(Ptc);

            if (associatedAdult is not null && associatedAdult.Value == Id)
                throw ExceptionFactory.AssociatedAdultMustNotBeSelf(Id);

            Name = name;
            DateOfBirth = dateOfBirth;
            AssociatedAdult = associatedAdult;
            CustomerRef = customerRef;
            _documents.Clear();
            _documents.AddRange(documents);
        }
    }
}
