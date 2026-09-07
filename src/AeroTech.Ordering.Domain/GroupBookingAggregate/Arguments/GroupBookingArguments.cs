using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Enums;
using AeroTech.Ordering.Domain._Shared.Identifiers;
using AeroTech.Ordering.Domain._Shared.ValueObjects;
using AeroTech.Ordering.Domain.OrderAggregate.ValueObjects;
using NodaTime;

namespace AeroTech.Ordering.Domain.GroupBookingAggregate.Arguments
{
    [SpecRef("CDR-2.2.4-Create")]
    public sealed record CreateGroupBookingArguments
    {
        [SpecRef("CDR-2.2.4-Create")]
        public required CurrencyCode SaleCurrency { get; init; }

        [SpecRef("CDR-2.2.4-Create")]
        public required IReadOnlyList<RequestedSeatBlock> Blocks { get; init; }
    }

    [SpecRef("CDR-2.2.2")]
    public sealed record RequestedSeatBlock
    {
        [SpecRef("CDR-2.2.2")]
        public required BlockId BlockId { get; init; }

        [SpecRef("CDR-2.2.2")]
        public required FlightSegmentRef Flight { get; init; }

        [SpecRef("CDR-2.2.2")]
        public required CabinCode Cabin { get; init; }

        [SpecRef("CDR-2.2.2")]
        public required RbdCode Rbd { get; init; }

        [SpecRef("CDR-2.2.2")]
        public required int SeatsHeld { get; init; }
    }

    [SpecRef("CDR-2.2.4-ConfirmBlocks")]
    public sealed record ConfirmBlocksArguments
    {
        [SpecRef("CDR-2.2.4-ConfirmBlocks")]
        public required IReadOnlyList<ConfirmedBlock> Blocks { get; init; }
    }

    [SpecRef("CDR-2.2.4-ConfirmBlocks")]
    public sealed record ConfirmedBlock
    {
        [SpecRef("CDR-2.2.4-ConfirmBlocks")]
        public required BlockId BlockId { get; init; }

        [SpecRef("CDR-2.2.4-ConfirmBlocks")]
        public required string InventoryBlockRef { get; init; }

        [SpecRef("CDR-2.2.3")]
        public required IReadOnlyList<SlotId> SlotIds { get; init; }
    }

    [SpecRef("CDR-2.2.4-RecordDeposit")]
    public sealed record RecordDepositArguments
    {
        [SpecRef("CDR-2.2.4-RecordDeposit")]
        public required PaymentRecordId PaymentRecordId { get; init; }

        [SpecRef("CDR-2.2.4-RecordDeposit")]
        public required FormOfPayment Fop { get; init; }

        [SpecRef("CDR-2.2.4-RecordDeposit")]
        public required Money Amount { get; init; }

        [SpecRef("CDR-2.2.4-RecordDeposit")]
        public PaymentRequestRef? RequestRef { get; init; }

        [SpecRef("CDR-2.2.4-RecordDeposit")]
        public FxSnapshot? Fx { get; init; }

        [SpecRef("CDR-2.2.4-RecordDeposit")]
        public string? PayerRef { get; init; }
    }

    [SpecRef("CDR-2.2.4-AllocateName")]
    public sealed record AllocateNameArguments
    {
        [SpecRef("CDR-2.2.4-AllocateName")]
        public required SlotId SlotId { get; init; }

        [SpecRef("CDR-2.2.4-AllocateName")]
        public required TravelerName Name { get; init; }

        [SpecRef("CDR-2.2.4-AllocateName")]
        public required PassengerTypeCode Ptc { get; init; }

        [SpecRef("CDR-2.2.4-AllocateName")]
        public required OrderId SpawnedOrderRef { get; init; }

        [SpecRef("CDR-2.2.2")]
        public required IReadOnlyList<BlockId> BlockIds { get; init; }
    }

    [SpecRef("CDR-2.2.4-DeallocateName")]
    public sealed record DeallocateNameArguments
    {
        [SpecRef("CDR-2.2.4-DeallocateName")]
        public required SlotId SlotId { get; init; }

        [SpecRef("CDR-2.2.2")]
        public required IReadOnlyList<BlockId> BlockIds { get; init; }
    }

    [SpecRef("CDR-2.2.4-ReleaseSeats")]
    public sealed record ReleaseSeatsArguments
    {
        [SpecRef("CDR-2.2.4-ReleaseSeats")]
        public required BlockId BlockId { get; init; }

        [SpecRef("CDR-2.2.4-ReleaseSeats")]
        public required int Seats { get; init; }

        [SpecRef("CDR-2.2.3")]
        public required IReadOnlyList<SlotId> SlotIds { get; init; }
    }

    [SpecRef("CDR-2.2.4-ExtendNameDeadline")]
    public sealed record ExtendNameDeadlineArguments
    {
        [SpecRef("CDR-2.2.4-ExtendNameDeadline")]
        public required TimeLimitId TimeLimitId { get; init; }

        [SpecRef("CDR-2.2.4-ExtendNameDeadline")]
        public required Instant DueAtUtc { get; init; }
    }

    [SpecRef("CDR-2.1.7")]
    public sealed record SetGroupTimeLimitArguments
    {
        [SpecRef("CDR-2.1.7")]
        public required TimeLimitId TimeLimitId { get; init; }

        [SpecRef("CDR-2.1.7")]
        public required TimeLimitType Type { get; init; }

        [SpecRef("CDR-2.1.7")]
        public required Instant DueAtUtc { get; init; }
    }

    [SpecRef("CDR-2.2.4-CancelGroup")]
    public sealed record CancelGroupArguments
    {
        [SpecRef("CDR-2.2.4-CancelGroup")]
        public required CancellationReason Reason { get; init; }
    }
}
