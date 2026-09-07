using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Contracts;
using AeroTech.Ordering.Domain._Shared.Resources;

namespace AeroTech.Ordering.Domain._Shared.ValueObjects
{
    [SpecRef("DD-VO-013")]
    public sealed record TravelerName
    {
#pragma warning disable CS8618
        private TravelerName()
        {
        }
#pragma warning restore CS8618

        [SpecRef("DD-VO-013")]
        public TravelerName(
            string given,
            string surname,
            string? title,
            string? givenLocal,
            string? surnameLocal,
            IDomainLimits limits)
        {
            if (string.IsNullOrWhiteSpace(given))
                throw ExceptionFactory.GivenNameIsRequired();

            if (string.IsNullOrWhiteSpace(surname))
                throw ExceptionFactory.SurnameIsRequired();

            EnsureWithinLength(given, limits);
            EnsureWithinLength(surname, limits);
            EnsureWithinLength(givenLocal, limits);
            EnsureWithinLength(surnameLocal, limits);

            Given = given;
            Surname = surname;
            Title = title;
            GivenLocal = givenLocal;
            SurnameLocal = surnameLocal;
        }

        [SpecRef("DD-VO-013")]
        public string Given { get; }

        [SpecRef("DD-VO-013")]
        public string Surname { get; }

        [SpecRef("DD-VO-013")]
        public string? Title { get; }

        [SpecRef("DD-VO-013")]
        public string? GivenLocal { get; }

        [SpecRef("DD-VO-013")]
        public string? SurnameLocal { get; }

        private static void EnsureWithinLength(string? value, IDomainLimits limits)
        {
            if (value is not null && value.Length > limits.MaxNameLength)
                throw ExceptionFactory.NameIsTooLong(limits.MaxNameLength);
        }
    }
}
