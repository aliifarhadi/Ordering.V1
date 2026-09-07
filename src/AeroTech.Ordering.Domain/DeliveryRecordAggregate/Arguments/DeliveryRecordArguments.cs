using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Enums;
using AeroTech.Ordering.Domain._Shared.Identifiers;
using AeroTech.Ordering.Domain._Shared.ValueObjects;
using AeroTech.Ordering.Domain.DeliveryRecordAggregate.ValueObjects;

namespace AeroTech.Ordering.Domain.DeliveryRecordAggregate.Arguments
{
    [SpecRef("CDR-3.1.4-Issue")]
    public sealed record IssueDeliveryRecordArguments
    {
        [SpecRef("CDR-3.1.1")]
        public required OrderId OrderRef { get; init; }

        [SpecRef("CDR-3.1.1")]
        public required TravelerId TravelerRef { get; init; }

        [SpecRef("CDR-3.1.1")]
        public required TravelerSnapshot TravelerSnapshot { get; init; }

        [SpecRef("CDR-3.1.1")]
        public required BranchId BranchRef { get; init; }

        [SpecRef("I-33")]
        public required int VoidWindowHours { get; init; }

        [SpecRef("CDR-3.1.2")]
        public required IReadOnlyList<IssuedSegmentUnit> SegmentUnits { get; init; }

        [SpecRef("CDR-3.1.3")]
        public required IReadOnlyList<IssuedServiceUnit> ServiceUnits { get; init; }
    }

    [SpecRef("CDR-3.1.2")]
    public sealed record IssuedSegmentUnit
    {
        [SpecRef("CDR-3.1.2")]
        public required OrderItemId OrderItemRef { get; init; }

        [SpecRef("CDR-3.1.2")]
        public required JourneyElementId JourneyRef { get; init; }

        [SpecRef("CDR-3.1.2")]
        public required FlightSegmentRef Flight { get; init; }

        [SpecRef("I-34")]
        public required Money UnitValue { get; init; }
    }

    [SpecRef("CDR-3.1.3")]
    public sealed record IssuedServiceUnit
    {
        [SpecRef("CDR-3.1.3")]
        public required OrderItemId OrderItemRef { get; init; }

        [SpecRef("CDR-3.1.3")]
        public required string ServiceCode { get; init; }

        [SpecRef("CDR-3.1.3")]
        public required string Description { get; init; }

        [SpecRef("CDR-3.1.3")]
        public required Money UnitValue { get; init; }

        [SpecRef("CDR-3.1.3")]
        public int? AssociatedSegmentUnit { get; init; }
    }

    [SpecRef("CDR-3.1.4-TransferControl")]
    public sealed record TransferControlArguments
    {
        [SpecRef("CDR-3.1.4-TransferControl")]
        public required int UnitNumber { get; init; }

        [SpecRef("CDR-3.4-ControlLease")]
        public required ControlLease Lease { get; init; }
    }

    [SpecRef("CDR-3.1.4-ReleaseControl")]
    public sealed record ReleaseControlArguments
    {
        [SpecRef("CDR-3.1.4-ReleaseControl")]
        public required int UnitNumber { get; init; }
    }

    [SpecRef("CDR-3.1.4-ApplyUnitStatusChange")]
    public sealed record ApplyUnitStatusChangeArguments
    {
        [SpecRef("CDR-3.1.4-ApplyUnitStatusChange")]
        public required int UnitNumber { get; init; }

        [SpecRef("CDR-3.1.4-ApplyUnitStatusChange")]
        public required SegmentDeliveryStatus Status { get; init; }

        [SpecRef("CDR-3.4-UnitStatusChange")]
        public required string Source { get; init; }
    }

    [SpecRef("CDR-3.1.6")]
    public sealed record ConsumeServiceUnitArguments
    {
        [SpecRef("CDR-3.1.6")]
        public required int UnitNumber { get; init; }
    }

    [SpecRef("CDR-3.1.4-VoidRecord")]
    public sealed record VoidRecordArguments
    {
        [SpecRef("CDR-3.4-UnitStatusChange")]
        public required string Source { get; init; }
    }

    [SpecRef("CDR-3.1.4-RefundUnits")]
    public sealed record RefundUnitsArguments
    {
        [SpecRef("CDR-3.1.4-RefundUnits")]
        public required IReadOnlyList<int> SegmentUnitNumbers { get; init; }

        [SpecRef("CDR-3.1.4-RefundUnits")]
        public required IReadOnlyList<int> ServiceUnitNumbers { get; init; }

        [SpecRef("CDR-3.4-UnitStatusChange")]
        public required string Source { get; init; }
    }

    [SpecRef("CDR-3.1.4-CorrectUnitStatus")]
    public sealed record CorrectUnitStatusArguments
    {
        [SpecRef("CDR-3.1.4-CorrectUnitStatus")]
        public required int UnitNumber { get; init; }

        [SpecRef("CDR-3.1.4-CorrectUnitStatus")]
        public required SegmentDeliveryStatus Status { get; init; }

        [SpecRef("CDR-3.1.4-CorrectUnitStatus")]
        public required string Justification { get; init; }
    }
}
