using AeroTech.Framework.Core.Domain.Aggregates;
using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Contracts;
using AeroTech.Ordering.Domain._Shared.Enums;
using AeroTech.Ordering.Domain._Shared.Identifiers;
using AeroTech.Ordering.Domain._Shared.Resources;
using AeroTech.Ordering.Domain._Shared.ValueObjects;
using AeroTech.Ordering.Domain.GroupBookingAggregate.Arguments;
using AeroTech.Ordering.Domain.GroupBookingAggregate.DomainEvents;
using AeroTech.Ordering.Domain.GroupBookingAggregate.Entities;
using AeroTech.Ordering.Domain.GroupBookingAggregate.Specifications;
using AeroTech.Ordering.Domain.OrderAggregate.Entities;
using NodaTime;
using IClock = AeroTech.Framework.Core.ServiceContracts.IClock;
using IIdGenerator = AeroTech.Framework.Core.ServiceContracts.IIdGenerator;

namespace AeroTech.Ordering.Domain.GroupBookingAggregate
{
    [SpecRef("CDR-2.2.1")]
    public sealed class GroupBooking : AggregateRoot<GroupBookingId>
    {
        private readonly List<SeatBlock> _blocks = [];
        private readonly List<NameSlot> _slots = [];
        private readonly List<PaymentRecord> _payments = [];
        private readonly List<TimeLimit> _timeLimits = [];
        private readonly List<OrderId> _spawnedOrders = [];

        private GroupBooking()
        {
        }

        private GroupBooking(
            GroupBookingId id,
            GroupReference reference,
            SalesContext salesContext)
        {
            Id = id;
            Reference = reference;
            SalesContext = salesContext;
            AggregateVersion = 0;
            IsSuspended = false;
            BlocksConfirmed = false;
        }

        [SpecRef("CDR-2.2.1")]
        public GroupReference Reference { get; private set; }

        [SpecRef("CDR-2.2.1")]
        public SalesContext SalesContext { get; private set; } = null!;

        [SpecRef("CDR-2.2.1")]
        public long AggregateVersion { get; private set; }

        [SpecRef("CDR-2.2.5")]
        public GroupStatus Status => GroupStatusSpecification.Derive(Blocks, Slots, BlocksConfirmed);

        [SpecRef("CDR-2.2.1")]
        public bool IsSuspended { get; private set; }

        [SpecRef("CDR-2.2.1")]
        public Instant? ClosedAtUtc { get; private set; }

        [SpecRef("CDR-2.2.4-ConfirmBlocks")]
        public bool BlocksConfirmed { get; private set; }

        [SpecRef("CDR-2.2.1")]
        public IReadOnlyList<SeatBlock> Blocks => _blocks.AsReadOnly();

        [SpecRef("CDR-2.2.1")]
        public IReadOnlyList<NameSlot> Slots => _slots.AsReadOnly();

        [SpecRef("CDR-2.2.1")]
        public IReadOnlyList<PaymentRecord> Payments => _payments.AsReadOnly();

        [SpecRef("CDR-2.2.1")]
        public IReadOnlyList<TimeLimit> TimeLimits => _timeLimits.AsReadOnly();

        [SpecRef("CDR-2.2.1")]
        public IReadOnlyList<OrderId> SpawnedOrders => _spawnedOrders.AsReadOnly();

        [SpecRef("CDR-2.2.4-Create")]
        public static GroupBooking Create(
            CreateGroupBookingArguments arguments,
            IAuthenticatedContext context,
            IEnabledCapabilities capabilities,
            IGroupReferenceGenerator references,
            IIdGenerator idGenerator,
            IClock clock)
        {
            if (!capabilities.IsChannelEnabled(context.Channel))
                throw ExceptionFactory.ChannelNotEnabled(context.Channel);

            var now = Instant.FromDateTimeOffset(clock.GetDateTime());

            var group = new GroupBooking(
                id: GroupBookingId.New(),
                reference: references.Next(),
                salesContext: SalesContext.FromAuthenticatedContext(context, arguments.SaleCurrency, now));

            foreach (var requested in arguments.Blocks)
            {
                if (group._blocks.Any(block => block.Id == requested.BlockId))
                    throw ExceptionFactory.DuplicateBlockId(requested.BlockId);

                group._blocks.Add(new SeatBlock(
                    id: requested.BlockId,
                    flight: requested.Flight,
                    cabin: requested.Cabin,
                    rbd: requested.Rbd,
                    seatsHeld: requested.SeatsHeld,
                    inventoryBlockRef: null));
            }

            group.AggregateVersion++;

            group.Causes(new GroupBookingCreated(
                EventId: idGenerator.NewId().ToString(),
                AggregateId: group.Id.ToString(),
                Reference: group.Reference,
                AggregateVersion: group.AggregateVersion,
                TimeOfOccurrence: clock.GetDateTime()));

            return group;
        }

        [SpecRef("CDR-2.2.4-ConfirmBlocks")]
        public void ConfirmBlocks(
            ConfirmBlocksArguments arguments,
            IAuthenticatedContext context,
            IIdGenerator idGenerator,
            IClock clock)
        {
            EnsureFlagsClear();
            EnsureServicingAuthority(context);

            foreach (var confirmed in arguments.Blocks)
            {
                RequireBlock(confirmed.BlockId).ConfirmInventoryBlock(confirmed.InventoryBlockRef);

                foreach (var slotId in confirmed.SlotIds)
                {
                    if (_slots.Any(slot => slot.Id == slotId))
                        throw ExceptionFactory.DuplicateSlotId(slotId);

                    _slots.Add(new NameSlot(slotId));
                }
            }

            EnsureSlotsDoNotExceedHeldBlock();

            BlocksConfirmed = true;
            AggregateVersion++;

            Causes(new GroupBlocksConfirmed(
                EventId: idGenerator.NewId().ToString(),
                AggregateId: Id.ToString(),
                AggregateVersion: AggregateVersion,
                TimeOfOccurrence: clock.GetDateTime()));
        }

        [SpecRef("CDR-2.2.4-RecordDeposit")]
        public void RecordDeposit(
            RecordDepositArguments arguments,
            IAuthenticatedContext context,
            IEnabledCapabilities capabilities,
            IIdGenerator idGenerator,
            IClock clock)
        {
            EnsureFlagsClear();
            EnsureServicingAuthority(context);

            if (!capabilities.IsFormOfPaymentEnabled(arguments.Fop, SalesContext.Channel))
                throw ExceptionFactory.FormOfPaymentNotEnabled(arguments.Fop, SalesContext.Channel);

            if (_payments.Any(payment => payment.Id == arguments.PaymentRecordId))
                throw ExceptionFactory.DuplicatePaymentRecordId(arguments.PaymentRecordId);

            if (arguments.Amount.Currency != SalesContext.SaleCurrency)
                throw ExceptionFactory.SaleCurrencyImmutableAfterPayment();

            var now = Instant.FromDateTimeOffset(clock.GetDateTime());

            _payments.Add(new PaymentRecord(
                id: arguments.PaymentRecordId,
                fop: arguments.Fop,
                amount: arguments.Amount,
                requestRef: arguments.RequestRef,
                fx: arguments.Fx,
                payerRef: arguments.PayerRef,
                recordedAtUtc: now));

            AggregateVersion++;

            Causes(new GroupDepositRecorded(
                EventId: idGenerator.NewId().ToString(),
                AggregateId: Id.ToString(),
                PaymentRecordId: arguments.PaymentRecordId,
                AggregateVersion: AggregateVersion,
                TimeOfOccurrence: clock.GetDateTime()));
        }

        [SpecRef("CDR-2.2.4-AllocateName")]
        public void AllocateName(
            AllocateNameArguments arguments,
            IAuthenticatedContext context,
            IIdGenerator idGenerator,
            IClock clock)
        {
            EnsureFlagsClear();
            EnsureServicingAuthority(context);

            if (!BlocksConfirmed)
                throw ExceptionFactory.BlocksMustBeConfirmedBeforeAllocation();

            var slot = RequireSlot(arguments.SlotId);

            if (slot.Status != NameSlotStatus.Unallocated)
                throw ExceptionFactory.SlotIsNotUnallocated(slot.Id);

            foreach (var blockId in arguments.BlockIds)
                RequireBlock(blockId).Allocate(1);

            slot.Allocate(arguments.Name, arguments.Ptc, arguments.SpawnedOrderRef);
            _spawnedOrders.Add(arguments.SpawnedOrderRef);

            EnsureSlotsDoNotExceedHeldBlock();

            AggregateVersion++;

            Causes(new GroupNameAllocated(
                EventId: idGenerator.NewId().ToString(),
                AggregateId: Id.ToString(),
                SlotId: slot.Id,
                SpawnedOrderRef: arguments.SpawnedOrderRef,
                AggregateVersion: AggregateVersion,
                TimeOfOccurrence: clock.GetDateTime()));
        }

        [SpecRef("CDR-2.2.4-DeallocateName")]
        public void DeallocateName(
            DeallocateNameArguments arguments,
            IAuthenticatedContext context,
            IIdGenerator idGenerator,
            IClock clock)
        {
            EnsureFlagsClear();
            EnsureServicingAuthority(context);

            var slot = RequireSlot(arguments.SlotId);

            if (slot.Status != NameSlotStatus.Allocated)
                throw ExceptionFactory.SlotIsNotAllocated(slot.Id);

            var spawnedOrder = slot.OrderRef;

            foreach (var blockId in arguments.BlockIds)
                RequireBlock(blockId).Deallocate(1);

            slot.Deallocate();

            if (spawnedOrder is not null)
                _spawnedOrders.Remove(spawnedOrder.Value);

            AggregateVersion++;

            Causes(new GroupNameDeallocated(
                EventId: idGenerator.NewId().ToString(),
                AggregateId: Id.ToString(),
                SlotId: slot.Id,
                AggregateVersion: AggregateVersion,
                TimeOfOccurrence: clock.GetDateTime()));
        }

        [SpecRef("CDR-2.2.4-ReleaseSeats")]
        public void ReleaseSeats(
            ReleaseSeatsArguments arguments,
            IAuthenticatedContext context,
            IIdGenerator idGenerator,
            IClock clock)
        {
            EnsureFlagsClear();
            EnsureServicingAuthority(context);

            RequireBlock(arguments.BlockId).Release(arguments.Seats);

            foreach (var slotId in arguments.SlotIds)
                RequireSlot(slotId).Release();

            AggregateVersion++;

            Causes(new GroupSeatsReleased(
                EventId: idGenerator.NewId().ToString(),
                AggregateId: Id.ToString(),
                BlockId: arguments.BlockId,
                Seats: arguments.Seats,
                AggregateVersion: AggregateVersion,
                TimeOfOccurrence: clock.GetDateTime()));
        }

        [SpecRef("CDR-2.1.7")]
        public void SetTimeLimit(
            SetGroupTimeLimitArguments arguments,
            IAuthenticatedContext context,
            IClock clock)
        {
            EnsureFlagsClear();
            EnsureServicingAuthority(context);

            var now = Instant.FromDateTimeOffset(clock.GetDateTime());

            _timeLimits.Add(new TimeLimit(
                id: arguments.TimeLimitId,
                type: arguments.Type,
                dueAtUtc: arguments.DueAtUtc,
                appliesTo: [],
                now: now));

            AggregateVersion++;
        }

        [SpecRef("CDR-2.2.4-ExtendNameDeadline")]
        public void ExtendNameDeadline(
            ExtendNameDeadlineArguments arguments,
            IAuthenticatedContext context,
            IDomainLimits limits,
            IIdGenerator idGenerator,
            IClock clock)
        {
            EnsureFlagsClear();
            EnsureServicingAuthority(context);

            var timeLimit = RequireTimeLimit(arguments.TimeLimitId);
            var now = Instant.FromDateTimeOffset(clock.GetDateTime());

            timeLimit.Extend(arguments.DueAtUtc, limits, now);

            AggregateVersion++;

            Causes(new GroupTimeLimitExtended(
                EventId: idGenerator.NewId().ToString(),
                AggregateId: Id.ToString(),
                TimeLimitId: timeLimit.Id,
                AggregateVersion: AggregateVersion,
                TimeOfOccurrence: clock.GetDateTime()));
        }

        [SpecRef("CDR-2.2.4-CancelGroup")]
        public void CancelGroup(
            CancelGroupArguments arguments,
            IAuthenticatedContext context,
            IIdGenerator idGenerator,
            IClock clock)
        {
            EnsureFlagsClear();
            EnsureServicingAuthority(context);

            foreach (var slot in _slots.Where(slot => slot.Status == NameSlotStatus.Unallocated))
                slot.Release();

            foreach (var block in _blocks.Where(block => block.SeatsAvailable > 0))
                block.Release(block.SeatsAvailable);

            foreach (var timeLimit in _timeLimits.Where(timeLimit => timeLimit.IsActive))
                timeLimit.MarkCancelled();

            ClosedAtUtc = Instant.FromDateTimeOffset(clock.GetDateTime());
            AggregateVersion++;

            Causes(new GroupCancelled(
                EventId: idGenerator.NewId().ToString(),
                AggregateId: Id.ToString(),
                Reason: arguments.Reason,
                AggregateVersion: AggregateVersion,
                TimeOfOccurrence: clock.GetDateTime()));
        }

        [SpecRef("CDR-2.2.1")]
        public void Suspend(IAuthenticatedContext context)
        {
            EnsureServicingAuthority(context);
            IsSuspended = true;
            AggregateVersion++;
        }

        [SpecRef("CDR-2.2.1")]
        public void Unsuspend(IAuthenticatedContext context)
        {
            EnsureServicingAuthority(context);
            IsSuspended = false;
            AggregateVersion++;
        }

        [SpecRef("I-29")]
        private void EnsureSlotsDoNotExceedHeldBlock()
        {
            var held = _blocks.Sum(block => block.SeatsHeld - block.SeatsReleased);
            var allocated = _slots.Count(slot => slot.Status == NameSlotStatus.Allocated);

            if (allocated > held)
                throw ExceptionFactory.AllocatedSlotsCannotExceedHeldBlock(held);
        }

        private void EnsureFlagsClear()
        {
            if (IsSuspended)
                throw ExceptionFactory.GroupIsSuspended();

            if (ClosedAtUtc is not null)
                throw ExceptionFactory.GroupIsClosed();
        }

        [SpecRef("I-24")]
        private void EnsureServicingAuthority(IAuthenticatedContext context)
        {
            if (context.Actor.HasAirlineOverride)
                return;

            if (context.SellerId != SalesContext.SellerId)
                throw ExceptionFactory.ServicingRequiresMatchingSellerOrOverride();
        }

        private SeatBlock RequireBlock(BlockId blockId) =>
            _blocks.FirstOrDefault(block => block.Id == blockId)
            ?? throw ExceptionFactory.SeatBlockNotFound(blockId);

        private NameSlot RequireSlot(SlotId slotId) =>
            _slots.FirstOrDefault(slot => slot.Id == slotId)
            ?? throw ExceptionFactory.NameSlotNotFound(slotId);

        private TimeLimit RequireTimeLimit(TimeLimitId timeLimitId) =>
            _timeLimits.FirstOrDefault(timeLimit => timeLimit.Id == timeLimitId)
            ?? throw ExceptionFactory.TimeLimitNotFound(timeLimitId);
    }
}
