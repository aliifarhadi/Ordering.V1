using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Enums;
using AeroTech.Ordering.Domain.GroupBookingAggregate.Entities;

namespace AeroTech.Ordering.Domain.GroupBookingAggregate.Specifications
{
    [SpecRef("CDR-2.2.5")]
    public static class GroupStatusSpecification
    {
        [SpecRef("CDR-2.2.5")]
        public static GroupStatus Derive(
            IReadOnlyList<SeatBlock> blocks,
            IReadOnlyList<NameSlot> slots,
            bool blocksConfirmed)
        {
            if (blocks.Count > 0 &&
                blocks.All(block => block.SeatsAvailable == 0 && block.SeatsAllocated == 0) &&
                slots.All(slot => slot.Status == NameSlotStatus.Released))
                return GroupStatus.Cancelled;

            if (slots.Count > 0 &&
                slots.Any(slot => slot.Status == NameSlotStatus.Released) &&
                slots.All(slot => slot.Status != NameSlotStatus.Unallocated))
                return GroupStatus.Released;

            if (slots.Count > 0 && slots.All(slot => slot.Status == NameSlotStatus.Allocated))
                return GroupStatus.FullyAllocated;

            if (slots.Any(slot => slot.Status == NameSlotStatus.Allocated) &&
                slots.Any(slot => slot.Status != NameSlotStatus.Allocated))
                return GroupStatus.PartiallyAllocated;

            if (blocksConfirmed && slots.All(slot => slot.Status != NameSlotStatus.Allocated))
                return GroupStatus.Held;

            return GroupStatus.Draft;
        }
    }
}
