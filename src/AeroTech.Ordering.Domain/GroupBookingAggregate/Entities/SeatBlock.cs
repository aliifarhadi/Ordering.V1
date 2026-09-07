using AeroTech.Framework.Core.Domain.Entities;
using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Identifiers;
using AeroTech.Ordering.Domain._Shared.Resources;
using AeroTech.Ordering.Domain._Shared.ValueObjects;

namespace AeroTech.Ordering.Domain.GroupBookingAggregate.Entities
{
    [SpecRef("CDR-2.2.2")]
    public sealed class SeatBlock : Entity<BlockId>
    {
        private SeatBlock()
        {
        }

        [SpecRef("CDR-2.2.2")]
        internal SeatBlock(
            BlockId id,
            FlightSegmentRef flight,
            CabinCode cabin,
            RbdCode rbd,
            int seatsHeld,
            string? inventoryBlockRef)
        {
            if (seatsHeld <= 0)
                throw ExceptionFactory.SeatsHeldMustBePositive();

            Id = id;
            Flight = flight;
            Cabin = cabin;
            Rbd = rbd;
            SeatsHeld = seatsHeld;
            SeatsAllocated = 0;
            SeatsReleased = 0;
            InventoryBlockRef = inventoryBlockRef;
        }

        [SpecRef("CDR-2.2.2")]
        public FlightSegmentRef Flight { get; private set; } = null!;

        [SpecRef("CDR-2.2.2")]
        public CabinCode Cabin { get; private set; }

        [SpecRef("CDR-2.2.2")]
        public RbdCode Rbd { get; private set; }

        [SpecRef("CDR-2.2.2")]
        public int SeatsHeld { get; private set; }

        [SpecRef("CDR-2.2.2")]
        public int SeatsAllocated { get; private set; }

        [SpecRef("CDR-2.2.2")]
        public int SeatsReleased { get; private set; }

        [SpecRef("CDR-2.2.2")]
        public string? InventoryBlockRef { get; private set; }

        [SpecRef("CDR-2.2.2")]
        public int SeatsAvailable => SeatsHeld - SeatsAllocated - SeatsReleased;

        [SpecRef("CDR-2.2.4-ConfirmBlocks")]
        internal void ConfirmInventoryBlock(string inventoryBlockRef)
        {
            if (string.IsNullOrWhiteSpace(inventoryBlockRef))
                throw ExceptionFactory.IdentifierIsRequired(nameof(inventoryBlockRef));

            InventoryBlockRef = inventoryBlockRef;
        }

        [SpecRef("I-29")]
        internal void Allocate(int seats)
        {
            if (SeatsAllocated + seats > SeatsHeld - SeatsReleased)
                throw ExceptionFactory.AllocatedSlotsCannotExceedHeldBlock(SeatsHeld);

            SeatsAllocated += seats;
        }

        [SpecRef("CDR-2.2.4-DeallocateName")]
        internal void Deallocate(int seats)
        {
            SeatsAllocated -= seats;
        }

        [SpecRef("I-28")]
        internal void Release(int seats)
        {
            if (seats > SeatsAvailable)
                throw ExceptionFactory.CannotReleaseMoreSeatsThanHeld(Id);

            SeatsReleased += seats;
        }
    }
}
