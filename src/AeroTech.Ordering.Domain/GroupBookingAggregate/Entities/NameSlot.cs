using AeroTech.Framework.Core.Domain.Entities;
using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Enums;
using AeroTech.Ordering.Domain._Shared.Identifiers;
using AeroTech.Ordering.Domain._Shared.Resources;
using AeroTech.Ordering.Domain._Shared.ValueObjects;

namespace AeroTech.Ordering.Domain.GroupBookingAggregate.Entities
{
    [SpecRef("CDR-2.2.3")]
    public sealed class NameSlot : Entity<SlotId>
    {
        private NameSlot()
        {
        }

        [SpecRef("CDR-2.2.3")]
        internal NameSlot(SlotId id)
        {
            Id = id;
            Status = NameSlotStatus.Unallocated;
        }

        [SpecRef("CDR-2.2.3")]
        public NameSlotStatus Status { get; private set; }

        [SpecRef("CDR-2.2.3")]
        public TravelerName? Name { get; private set; }

        [SpecRef("CDR-2.2.3")]
        public PassengerTypeCode? Ptc { get; private set; }

        [SpecRef("CDR-2.2.3")]
        public OrderId? OrderRef { get; private set; }

        [SpecRef("CDR-2.2.4-AllocateName")]
        internal void Allocate(TravelerName name, PassengerTypeCode ptc, OrderId orderRef)
        {
            TransitionTo(NameSlotStatus.Allocated);

            Name = name;
            Ptc = ptc;
            OrderRef = orderRef;
        }

        [SpecRef("CDR-2.2.4-DeallocateName")]
        internal void Deallocate()
        {
            TransitionTo(NameSlotStatus.Unallocated);

            Name = null;
            Ptc = null;
            OrderRef = null;
        }

        [SpecRef("CDR-2.2.4-ReleaseSeats")]
        internal void Release() => TransitionTo(NameSlotStatus.Released);

        private void TransitionTo(NameSlotStatus target)
        {
            if (!IsTransitionPermitted(Status, target))
                throw ExceptionFactory.SlotCannotTransition(Id, Status, target);

            Status = target;
        }

        [SpecRef("CDR-2.2.6")]
        private static bool IsTransitionPermitted(NameSlotStatus from, NameSlotStatus to) =>
            (from, to) switch
            {
                (NameSlotStatus.Unallocated, NameSlotStatus.Allocated) => true,
                (NameSlotStatus.Allocated, NameSlotStatus.Unallocated) => true,
                (NameSlotStatus.Unallocated, NameSlotStatus.Released) => true,
                _ => false,
            };
    }
}
