using AeroTech.Ordering.Domain._Shared.Attributes;

namespace AeroTech.Ordering.Domain._Shared.Enums
{
    [SpecRef("CDR-ENUM-SubmissionStatus")]
    public enum SubmissionStatus
    {
        [SpecRef("CDR-ENUM-SubmissionStatus")]
        NotRequired = 1,

        [SpecRef("CDR-ENUM-SubmissionStatus")]
        PendingSubmission = 2,

        [SpecRef("CDR-ENUM-SubmissionStatus")]
        Submitted = 3,

        [SpecRef("CDR-ENUM-SubmissionStatus")]
        Accepted = 4,

        [SpecRef("CDR-ENUM-SubmissionStatus")]
        Rejected = 5,
    }
}
