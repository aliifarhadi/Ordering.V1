using AeroTech.Framework.Core.Domain.Aggregates;
using AeroTech.Framework.Core.ServiceContracts;
using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Contracts;
using AeroTech.Ordering.Domain._Shared.Enums;
using AeroTech.Ordering.Domain._Shared.Identifiers;
using AeroTech.Ordering.Domain._Shared.Resources;
using AeroTech.Ordering.Domain._Shared.ValueObjects;
using AeroTech.Ordering.Domain.OrderAggregate.Arguments;
using AeroTech.Ordering.Domain.OrderAggregate.DomainEvents;
using AeroTech.Ordering.Domain.OrderAggregate.Entities;
using AeroTech.Ordering.Domain.OrderAggregate.Specifications;
using AeroTech.Ordering.Domain.OrderAggregate.ValueObjects;
using NodaTime;
using IClock = AeroTech.Framework.Core.ServiceContracts.IClock;

namespace AeroTech.Ordering.Domain.OrderAggregate
{
    [SpecRef("DD-AGG-001")]
    public sealed class Order : AggregateRoot<OrderId>
    {
        private readonly List<Traveler> _travelers = [];
        private readonly List<JourneyElement> _journeyElements = [];
        private readonly List<TravelerJourneySegment> _segments = [];
        private readonly List<OrderItem> _items = [];
        private readonly List<PaymentRecord> _payments = [];
        private readonly List<TimeLimit> _timeLimits = [];
        private readonly List<ContactPoint> _contacts = [];
        private readonly List<OrderHistoryEntry> _history = [];

        private Order()
        {
        }

        private Order(
            OrderId id,
            OrderReference reference,
            SalesContext salesContext)
        {
            Id = id;
            Reference = reference;
            SalesContext = salesContext;
            AggregateVersion = 0;
            IsSuspended = false;
            IsUnderLegalHold = false;
            RequiresReview = false;
        }

        [SpecRef("DD-AGG-001-02")]
        public OrderReference Reference { get; private set; }

        [SpecRef("CDR-2.1.1-GroupRef")]
        public GroupBookingId? GroupRef { get; private set; }

        [SpecRef("DD-AGG-001-03")]
        public SalesContext SalesContext { get; private set; } = null!;

        [SpecRef("DD-AGG-001-04")]
        public long AggregateVersion { get; private set; }

        [SpecRef("DD-AGG-001-05")]
        public OrderStatus Status => OrderStatusSpecification.Derive(Items, ClosedAtUtc, Totals.BalanceDue);

        [SpecRef("DD-AGG-001-06")]
        public OrderTotals Totals => ComputeTotals();

        [SpecRef("DD-AGG-001-07")]
        public Instant? ClosedAtUtc { get; private set; }

        [SpecRef("DD-AGG-001-08")]
        public bool IsSuspended { get; private set; }

        [SpecRef("DD-AGG-001-09")]
        public bool IsUnderLegalHold { get; private set; }

        [SpecRef("DD-AGG-001-10")]
        public bool RequiresReview { get; private set; }

        [SpecRef("DD-AGG-001-11")]
        public IReadOnlyList<Traveler> Travelers => _travelers.AsReadOnly();

        [SpecRef("DD-AGG-001-12")]
        public IReadOnlyList<JourneyElement> JourneyElements => _journeyElements.AsReadOnly();

        [SpecRef("DD-AGG-001-13")]
        public IReadOnlyList<TravelerJourneySegment> Segments => _segments.AsReadOnly();

        [SpecRef("DD-AGG-001-14")]
        public IReadOnlyList<OrderItem> Items => _items.AsReadOnly();

        [SpecRef("DD-AGG-001-15")]
        public IReadOnlyList<PaymentRecord> Payments => _payments.AsReadOnly();

        [SpecRef("DD-AGG-001-16")]
        public IReadOnlyList<TimeLimit> TimeLimits => _timeLimits.AsReadOnly();

        [SpecRef("DD-AGG-001-17")]
        public IReadOnlyList<ContactPoint> Contacts => _contacts.AsReadOnly();

        [SpecRef("DD-AGG-001-18")]
        public IReadOnlyList<OrderHistoryEntry> History => _history.AsReadOnly();

        [SpecRef("DD-CMD-001")]
        public static Order Create(
            CreateOrderArguments arguments,
            IAuthenticatedContext context,
            IEnabledCapabilities capabilities,
            IOrderReferenceGenerator references,
            IIdGenerator idGenerator,
            IClock clock)
        {
            if (!capabilities.IsChannelEnabled(context.Channel))
                throw ExceptionFactory.ChannelNotEnabled(context.Channel);

            var now = Instant.FromDateTimeOffset(clock.GetDateTime());

            var order = new Order(
                id: OrderId.New(),
                reference: references.Next(),
                salesContext: SalesContext.FromAuthenticatedContext(context, arguments.SaleCurrency, now));

            order.AppendHistory(context, nameof(Create), now);
            order.AggregateVersion++;

            order.Causes(new OrderCreated(
                EventId: idGenerator.NewId().ToString(),
                AggregateId: order.Id.ToString(),
                Reference: order.Reference,
                AggregateVersion: order.AggregateVersion,
                TimeOfOccurrence: clock.GetDateTime()));

            return order;
        }

        [SpecRef("DD-CMD-002")]
        public void AddTraveler(
            AddTravelerArguments arguments,
            IAuthenticatedContext context,
            IDomainLimits limits,
            IIdGenerator idGenerator,
            IClock clock)
        {
            EnsureFlagsClear();
            EnsureServicingAuthority(context);

            if (_travelers.Any(traveler => traveler.Id == arguments.TravelerId))
                throw ExceptionFactory.DuplicateTravelerId(arguments.TravelerId);

            if (_travelers.Count >= limits.MaxTravelersPerOrder)
                throw ExceptionFactory.TravelerLimitExceeded(limits.MaxTravelersPerOrder);

            EnsureAssociatedAdultIsValid(arguments.Ptc, arguments.TravelerId, arguments.AssociatedAdult);

            _travelers.Add(new Traveler(
                id: arguments.TravelerId,
                name: arguments.Name,
                ptc: arguments.Ptc,
                dateOfBirth: arguments.DateOfBirth,
                documents: arguments.Documents,
                associatedAdult: arguments.AssociatedAdult,
                customerRef: arguments.CustomerRef));

            var now = Instant.FromDateTimeOffset(clock.GetDateTime());
            AppendHistory(context, nameof(AddTraveler), now);
            AggregateVersion++;

            Causes(new TravelerAdded(
                EventId: idGenerator.NewId().ToString(),
                AggregateId: Id.ToString(),
                TravelerId: arguments.TravelerId,
                AggregateVersion: AggregateVersion,
                TimeOfOccurrence: clock.GetDateTime()));
        }

        [SpecRef("DD-CMD-003")]
        public void UpdateTravelerDetails(
            UpdateTravelerDetailsArguments arguments,
            IAuthenticatedContext context,
            IIdGenerator idGenerator,
            IClock clock)
        {
            EnsureFlagsClear();
            EnsureServicingAuthority(context);

            var traveler = RequireTraveler(arguments.TravelerId);

            EnsureAssociatedAdultIsValid(traveler.Ptc, traveler.Id, arguments.AssociatedAdult);

            traveler.UpdateDetails(
                name: arguments.Name,
                dateOfBirth: arguments.DateOfBirth,
                documents: arguments.Documents,
                associatedAdult: arguments.AssociatedAdult,
                customerRef: arguments.CustomerRef);

            var now = Instant.FromDateTimeOffset(clock.GetDateTime());
            AppendHistory(context, nameof(UpdateTravelerDetails), now);
            AggregateVersion++;

            Causes(new TravelerUpdated(
                EventId: idGenerator.NewId().ToString(),
                AggregateId: Id.ToString(),
                TravelerId: traveler.Id,
                AggregateVersion: AggregateVersion,
                TimeOfOccurrence: clock.GetDateTime()));
        }

        [SpecRef("DD-CMD-004")]
        public void AddContactPoint(
            ContactPointArguments arguments,
            IAuthenticatedContext context,
            IIdGenerator idGenerator,
            IClock clock)
        {
            EnsureFlagsClear();
            EnsureServicingAuthority(context);

            if (arguments.Contact.IsPrimary &&
                _contacts.Any(contact => contact.Type == arguments.Contact.Type && contact.IsPrimary))
                throw ExceptionFactory.OnlyOnePrimaryContactPerType(arguments.Contact.Type);

            _contacts.Add(arguments.Contact);

            RecordContactChange(context, nameof(AddContactPoint), idGenerator, clock);
        }

        [SpecRef("DD-CMD-004")]
        public void RemoveContactPoint(
            ContactPointArguments arguments,
            IAuthenticatedContext context,
            IIdGenerator idGenerator,
            IClock clock)
        {
            EnsureFlagsClear();
            EnsureServicingAuthority(context);

            _contacts.Remove(arguments.Contact);

            RecordContactChange(context, nameof(RemoveContactPoint), idGenerator, clock);
        }

        [SpecRef("DD-CMD-005")]
        public void AddOrderItem(
            AddOrderItemArguments arguments,
            IAuthenticatedContext context,
            IDomainLimits limits,
            IEnabledCapabilities capabilities,
            IIdGenerator idGenerator,
            IClock clock)
        {
            EnsureFlagsClear();
            EnsureServicingAuthority(context);

            if (!capabilities.IsItemTypeEnabled(arguments.Type))
                throw ExceptionFactory.ItemTypeNotEnabled(arguments.Type);

            if (_items.Any(item => item.Id == arguments.OrderItemId))
                throw ExceptionFactory.DuplicateOrderItemId(arguments.OrderItemId);

            if (_items.Count >= limits.MaxItemsPerOrder)
                throw ExceptionFactory.ItemLimitExceeded(limits.MaxItemsPerOrder);

            if (_travelers.All(traveler => traveler.Id != arguments.TravelerRef))
                throw ExceptionFactory.ItemRequiresExistingTraveler(arguments.OrderItemId, arguments.TravelerRef);

            foreach (var journeyRef in arguments.JourneyRefs)
            {
                if (_journeyElements.All(element => element.Id != journeyRef))
                    throw ExceptionFactory.JourneyElementNotFound(journeyRef);
            }

            if (arguments.Type != ItemType.Flight)
                EnsureAncillaryHasConfirmedFlightItem(arguments.TravelerRef, arguments.JourneyRefs);

            _items.Add(new OrderItem(
                id: arguments.OrderItemId,
                type: arguments.Type,
                travelerRef: arguments.TravelerRef,
                journeyRefs: arguments.JourneyRefs,
                product: arguments.Product,
                price: arguments.Price,
                sourceOffer: arguments.SourceOffer,
                replacesItem: arguments.ReplacesItem,
                partner: arguments.Partner));

            var now = Instant.FromDateTimeOffset(clock.GetDateTime());
            AppendHistory(context, nameof(AddOrderItem), now);
            AggregateVersion++;

            Causes(new OrderItemAdded(
                EventId: idGenerator.NewId().ToString(),
                AggregateId: Id.ToString(),
                OrderItemId: arguments.OrderItemId,
                ItemType: arguments.Type,
                AggregateVersion: AggregateVersion,
                TimeOfOccurrence: clock.GetDateTime()));
        }

        [SpecRef("DD-CMD-006")]
        public void Confirm(
            ConfirmOrderArguments arguments,
            IAuthenticatedContext context,
            IIdGenerator idGenerator,
            IClock clock)
        {
            EnsureFlagsClear();
            EnsureServicingAuthority(context);

            if (_travelers.Count == 0 || _items.Count == 0)
                throw ExceptionFactory.OrderRequiresTravelerAndItem();

            EnsureInfantsHaveAssociatedAdult();
            EnsureFlightItemsHaveSegmentPerJourney();

            var offered = _items.Where(item => item.Status == OrderItemStatus.Offered).ToList();
            foreach (var item in offered)
                item.MoveToPendingPayment();

            foreach (var segment in _segments.Where(segment => segment.SegmentStatus == SegmentStatus.Requested))
                segment.TransitionSegmentStatus(SegmentStatus.Confirmed);

            var now = Instant.FromDateTimeOffset(clock.GetDateTime());

            _timeLimits.Add(new TimeLimit(
                id: arguments.TimeLimitId,
                type: TimeLimitType.Payment,
                dueAtUtc: arguments.PaymentDueAtUtc,
                appliesTo: offered.Select(item => item.Id).ToList(),
                now: now));

            AppendHistory(context, nameof(Confirm), now);
            AggregateVersion++;

            Causes(new OrderConfirmed(
                EventId: idGenerator.NewId().ToString(),
                AggregateId: Id.ToString(),
                AggregateVersion: AggregateVersion,
                TimeOfOccurrence: clock.GetDateTime()));
        }

        [SpecRef("DD-CMD-007")]
        public void RecordPaymentRequest(
            RecordPaymentRequestArguments arguments,
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

            AppendHistory(context, nameof(RecordPaymentRequest), now);
            AggregateVersion++;

            Causes(new PaymentRequestRecorded(
                EventId: idGenerator.NewId().ToString(),
                AggregateId: Id.ToString(),
                PaymentRecordId: arguments.PaymentRecordId,
                AggregateVersion: AggregateVersion,
                TimeOfOccurrence: clock.GetDateTime()));
        }

        [SpecRef("DD-CMD-008")]
        public void ApplyPaymentResult(
            ApplyPaymentResultArguments arguments,
            IAuthenticatedContext context,
            IIdGenerator idGenerator,
            IClock clock)
        {
            EnsureFlagsClear();
            EnsureServicingAuthority(context);

            var payment = RequirePayment(arguments.PaymentRecordId);
            payment.ApplyResult(arguments.Status);

            var now = Instant.FromDateTimeOffset(clock.GetDateTime());
            AppendHistory(context, nameof(ApplyPaymentResult), now);
            AggregateVersion++;

            Causes(new PaymentResultApplied(
                EventId: idGenerator.NewId().ToString(),
                AggregateId: Id.ToString(),
                PaymentRecordId: payment.Id,
                Status: arguments.Status,
                AggregateVersion: AggregateVersion,
                TimeOfOccurrence: clock.GetDateTime()));
        }

        [SpecRef("DD-CMD-009")]
        public void AllocatePayment(
            AllocatePaymentArguments arguments,
            IAuthenticatedContext context,
            IIdGenerator idGenerator,
            IClock clock)
        {
            EnsureFlagsClear();
            EnsureServicingAuthority(context);

            var payment = RequirePayment(arguments.PaymentRecordId);

            foreach (var allocation in arguments.Allocations)
            {
                RequireItem(allocation.ItemRef);
                payment.Allocate(allocation);
            }

            foreach (var item in _items.Where(item => item.Status == OrderItemStatus.PendingPayment))
            {
                if (payment.IsSettled && IsItemBalanceSettled(item))
                    item.MarkConfirmed();
            }

            MarkCoveringTimeLimitsMet();

            var now = Instant.FromDateTimeOffset(clock.GetDateTime());
            AppendHistory(context, nameof(AllocatePayment), now);
            AggregateVersion++;

            Causes(new PaymentAllocated(
                EventId: idGenerator.NewId().ToString(),
                AggregateId: Id.ToString(),
                PaymentRecordId: payment.Id,
                AggregateVersion: AggregateVersion,
                TimeOfOccurrence: clock.GetDateTime()));
        }

        [SpecRef("DD-CMD-010")]
        public void RecordCreditAuthority(
            RecordCreditAuthorityArguments arguments,
            IAuthenticatedContext context,
            IIdGenerator idGenerator,
            IClock clock)
        {
            EnsureFlagsClear();
            EnsureServicingAuthority(context);

            foreach (var itemRef in arguments.ItemRefs)
                RequireItem(itemRef).RecordCreditAuthority(arguments.CreditAuthority);

            MarkCoveringTimeLimitsMet();

            var now = Instant.FromDateTimeOffset(clock.GetDateTime());
            AppendHistory(context, nameof(RecordCreditAuthority), now);
            AggregateVersion++;

            Causes(new CreditAuthorityRecorded(
                EventId: idGenerator.NewId().ToString(),
                AggregateId: Id.ToString(),
                AggregateVersion: AggregateVersion,
                TimeOfOccurrence: clock.GetDateTime()));
        }

        [SpecRef("DD-CMD-011")]
        public void CancelOrderItem(
            CancelOrderItemArguments arguments,
            IAuthenticatedContext context,
            IIdGenerator idGenerator,
            IClock clock)
        {
            EnsureFlagsClear();
            EnsureServicingAuthority(context);

            var item = RequireItem(arguments.OrderItemId);

            EnsureItemMayBeCancelled(item, arguments.DeliveryUnitsWithdrawn, arguments.ValueReturnCompletedOrReserved);

            item.MarkCancelled(arguments.Reason);
            CancelSegmentsWithoutActiveItems();

            var now = Instant.FromDateTimeOffset(clock.GetDateTime());
            AppendHistory(context, nameof(CancelOrderItem), now);
            AggregateVersion++;

            Causes(new OrderItemCancelled(
                EventId: idGenerator.NewId().ToString(),
                AggregateId: Id.ToString(),
                OrderItemId: item.Id,
                Reason: arguments.Reason,
                AggregateVersion: AggregateVersion,
                TimeOfOccurrence: clock.GetDateTime()));
        }

        [SpecRef("DD-CMD-012")]
        public void CancelOrder(
            CancelOrderArguments arguments,
            IAuthenticatedContext context,
            IIdGenerator idGenerator,
            IClock clock)
        {
            EnsureFlagsClear();
            EnsureServicingAuthority(context);

            var active = _items.Where(item => item.IsActive).ToList();

            foreach (var item in active)
                EnsureItemMayBeCancelled(item, arguments.DeliveryUnitsWithdrawn, arguments.ValueReturnCompletedOrReserved);

            foreach (var item in active)
                item.MarkCancelled(arguments.Reason);

            CancelSegmentsWithoutActiveItems();

            foreach (var timeLimit in _timeLimits.Where(timeLimit => timeLimit.IsActive))
                timeLimit.MarkCancelled();

            var now = Instant.FromDateTimeOffset(clock.GetDateTime());
            AppendHistory(context, nameof(CancelOrder), now);
            AggregateVersion++;

            Causes(new OrderCancelled(
                EventId: idGenerator.NewId().ToString(),
                AggregateId: Id.ToString(),
                Reason: arguments.Reason,
                AggregateVersion: AggregateVersion,
                TimeOfOccurrence: clock.GetDateTime()));
        }

        [SpecRef("DD-CMD-013")]
        public void ExtendTimeLimit(
            ExtendTimeLimitArguments arguments,
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

            AppendHistory(context, nameof(ExtendTimeLimit), now);
            AggregateVersion++;

            Causes(new TimeLimitExtended(
                EventId: idGenerator.NewId().ToString(),
                AggregateId: Id.ToString(),
                TimeLimitId: timeLimit.Id,
                AggregateVersion: AggregateVersion,
                TimeOfOccurrence: clock.GetDateTime()));
        }

        [SpecRef("DD-CMD-014")]
        public void ExpireTimeLimit(
            ExpireTimeLimitArguments arguments,
            IAuthenticatedContext context,
            IIdGenerator idGenerator,
            IClock clock)
        {
            EnsureFlagsClear();
            EnsureServicingAuthority(context);

            var timeLimit = RequireTimeLimit(arguments.TimeLimitId);

            foreach (var itemRef in timeLimit.AppliesTo)
            {
                if (arguments.ItemsCoveredByOpenPaymentCompletion.Contains(itemRef))
                    throw ExceptionFactory.ExpiryBlockedByOpenPaymentCompletion(itemRef);
            }

            var covered = timeLimit.AppliesTo
                .Select(RequireItem)
                .ToList();

            if (covered.Any(item => item.Status != OrderItemStatus.PendingPayment))
                throw ExceptionFactory.ExpiryOnlyAppliesToPendingPayment();

            timeLimit.MarkExpired();

            foreach (var item in covered)
                item.MarkCancelled(CancellationReason.TimeLimitExpiry);

            CancelSegmentsWithoutActiveItems();

            var now = Instant.FromDateTimeOffset(clock.GetDateTime());
            AppendHistory(context, nameof(ExpireTimeLimit), now);
            AggregateVersion++;

            Causes(new TimeLimitExpired(
                EventId: idGenerator.NewId().ToString(),
                AggregateId: Id.ToString(),
                TimeLimitId: timeLimit.Id,
                AggregateVersion: AggregateVersion,
                TimeOfOccurrence: clock.GetDateTime()));
        }

        [SpecRef("DD-CMD-015")]
        public void ApplyDeliveryResult(
            ApplyDeliveryResultArguments arguments,
            IAuthenticatedContext context,
            IIdGenerator idGenerator,
            IClock clock)
        {
            EnsureFlagsClear();
            EnsureServicingAuthority(context);

            foreach (var delivered in arguments.DeliveredItems)
            {
                var item = RequireItem(delivered.OrderItemId);
                item.MarkDelivered(IsItemBalanceSettled(item));

                RequireSegment(delivered.TravelerRef, delivered.JourneyRef)
                    .MirrorDeliveryRef(delivered.DeliveryRef);
            }

            var now = Instant.FromDateTimeOffset(clock.GetDateTime());
            AppendHistory(context, nameof(ApplyDeliveryResult), now);
            AggregateVersion++;

            Causes(new ItemsDelivered(
                EventId: idGenerator.NewId().ToString(),
                AggregateId: Id.ToString(),
                AggregateVersion: AggregateVersion,
                TimeOfOccurrence: clock.GetDateTime()));
        }

        [SpecRef("DD-CMD-016")]
        public void ApplyDeliveryStatusChange(
            ApplyDeliveryStatusChangeArguments arguments,
            IAuthenticatedContext context,
            IIdGenerator idGenerator,
            IClock clock)
        {
            var segment = RequireSegment(arguments.TravelerRef, arguments.JourneyRef);
            segment.MirrorDeliveryStatus(arguments.DeliveryStatus);

            if (segment.IsConsumed)
                MarkItemsConsumedFor(arguments.TravelerRef, arguments.JourneyRef);

            var now = Instant.FromDateTimeOffset(clock.GetDateTime());
            AppendHistory(context, nameof(ApplyDeliveryStatusChange), now);
            AggregateVersion++;

            Causes(new DeliveryStatusMirrored(
                EventId: idGenerator.NewId().ToString(),
                AggregateId: Id.ToString(),
                TravelerRef: arguments.TravelerRef,
                JourneyRef: arguments.JourneyRef,
                DeliveryStatus: arguments.DeliveryStatus,
                AggregateVersion: AggregateVersion,
                TimeOfOccurrence: clock.GetDateTime()));
        }

        [SpecRef("CDR-2.1.9-ApplyRefundResult")]
        public void ApplyRefundResult(
            ApplyRefundResultArguments arguments,
            IAuthenticatedContext context,
            IIdGenerator idGenerator,
            IClock clock)
        {
            EnsureFlagsClear();
            EnsureServicingAuthority(context);

            if (!arguments.DeliveryUnitsWithdrawn)
                throw ExceptionFactory.ValueReturnedBeforeUnitsWithdrawn(arguments.OrderItemId);

            var item = RequireItem(arguments.OrderItemId);
            item.MarkRefunded();

            var now = Instant.FromDateTimeOffset(clock.GetDateTime());
            AppendHistory(context, nameof(ApplyRefundResult), now);
            AggregateVersion++;

            Causes(new OrderItemRefunded(
                EventId: idGenerator.NewId().ToString(),
                AggregateId: Id.ToString(),
                OrderItemId: item.Id,
                AggregateVersion: AggregateVersion,
                TimeOfOccurrence: clock.GetDateTime()));
        }

        [SpecRef("DD-CMD-017")]
        public void RebookJourneyElement(
            RebookJourneyElementArguments arguments,
            IAuthenticatedContext context,
            IIdGenerator idGenerator,
            IClock clock)
        {
            EnsureFlagsClear();
            EnsureServicingAuthority(context);

            if (!arguments.NewSegmentConfirmed)
                throw ExceptionFactory.SegmentCannotTransition(SegmentStatus.Confirmed, SegmentStatus.Rebooked);

            var journeyElement = RequireJourneyElement(arguments.JourneyElementId);

            foreach (var segment in SegmentsOn(arguments.JourneyElementId))
            {
                EnsureSegmentNotUnderExternalControl(segment);
                segment.TransitionSegmentStatus(SegmentStatus.Rebooked);
            }

            journeyElement.Rebook(arguments.Flight, arguments.Cabin, arguments.Rbd);

            var now = Instant.FromDateTimeOffset(clock.GetDateTime());
            AppendHistory(context, nameof(RebookJourneyElement), now);
            AggregateVersion++;

            Causes(new JourneyElementRebooked(
                EventId: idGenerator.NewId().ToString(),
                AggregateId: Id.ToString(),
                JourneyElementId: journeyElement.Id,
                AggregateVersion: AggregateVersion,
                TimeOfOccurrence: clock.GetDateTime()));
        }

        [SpecRef("DD-CMD-018")]
        public void ApplyScheduleChange(
            ApplyScheduleChangeArguments arguments,
            IAuthenticatedContext context,
            IIdGenerator idGenerator,
            IClock clock)
        {
            EnsureFlagsClear();
            EnsureServicingAuthority(context);

            var journeyElement = RequireJourneyElement(arguments.JourneyElementId);
            journeyElement.ApplyScheduleChange(arguments.Flight);

            var now = Instant.FromDateTimeOffset(clock.GetDateTime());
            AppendHistory(context, nameof(ApplyScheduleChange), now);
            AggregateVersion++;

            Causes(new ScheduleChangeApplied(
                EventId: idGenerator.NewId().ToString(),
                AggregateId: Id.ToString(),
                JourneyElementId: journeyElement.Id,
                AggregateVersion: AggregateVersion,
                TimeOfOccurrence: clock.GetDateTime()));
        }

        [SpecRef("DD-CMD-019")]
        public void Suspend(
            SuspensionArguments arguments,
            IAuthenticatedContext context,
            IIdGenerator idGenerator,
            IClock clock)
        {
            EnsureServicingAuthority(context);
            IsSuspended = true;
            RecordSuspensionChange(context, nameof(Suspend), idGenerator, clock);
        }

        [SpecRef("DD-CMD-019")]
        public void Unsuspend(
            SuspensionArguments arguments,
            IAuthenticatedContext context,
            IIdGenerator idGenerator,
            IClock clock)
        {
            EnsureServicingAuthority(context);
            IsSuspended = false;
            RecordSuspensionChange(context, nameof(Unsuspend), idGenerator, clock);
        }

        [SpecRef("DD-CMD-020")]
        public void Close(
            IAuthenticatedContext context,
            IIdGenerator idGenerator,
            IClock clock)
        {
            EnsureServicingAuthority(context);

            if (IsUnderLegalHold)
                throw ExceptionFactory.OrderIsUnderLegalHold();

            var now = Instant.FromDateTimeOffset(clock.GetDateTime());
            ClosedAtUtc = now;

            AppendHistory(context, nameof(Close), now);
            AggregateVersion++;

            Causes(new OrderClosed(
                EventId: idGenerator.NewId().ToString(),
                AggregateId: Id.ToString(),
                AggregateVersion: AggregateVersion,
                TimeOfOccurrence: clock.GetDateTime()));
        }

        [SpecRef("DD-ENT-002")]
        public void AddJourneyElement(
            AddJourneyElementArguments arguments,
            IAuthenticatedContext context,
            IDomainLimits limits,
            IClock clock)
        {
            EnsureFlagsClear();
            EnsureServicingAuthority(context);

            if (_journeyElements.Any(element => element.Id == arguments.JourneyElementId))
                throw ExceptionFactory.DuplicateJourneyElementId(arguments.JourneyElementId);

            if (_journeyElements.Count >= limits.MaxJourneyElementsPerOrder)
                throw ExceptionFactory.JourneyElementLimitExceeded(limits.MaxJourneyElementsPerOrder);

            _journeyElements.Add(new JourneyElement(
                id: arguments.JourneyElementId,
                flight: arguments.Flight,
                cabin: arguments.Cabin,
                rbd: arguments.Rbd,
                marriedGroup: arguments.MarriedGroup,
                sequenceInJourney: arguments.SequenceInJourney));

            var now = Instant.FromDateTimeOffset(clock.GetDateTime());
            AppendHistory(context, nameof(AddJourneyElement), now);
            AggregateVersion++;
        }

        [SpecRef("DD-ENT-003")]
        public void AddSegment(
            TravelerId travelerRef,
            JourneyElementId journeyRef,
            IAuthenticatedContext context,
            IClock clock)
        {
            EnsureFlagsClear();
            EnsureServicingAuthority(context);

            if (_travelers.All(traveler => traveler.Id != travelerRef) ||
                _journeyElements.All(element => element.Id != journeyRef))
                throw ExceptionFactory.SegmentRequiresBothRefsPresent(travelerRef, journeyRef);

            if (_segments.Any(segment => segment.TravelerRef == travelerRef && segment.JourneyRef == journeyRef))
                throw ExceptionFactory.SegmentAlreadyExists(travelerRef, journeyRef);

            _segments.Add(new TravelerJourneySegment(travelerRef, journeyRef));

            var now = Instant.FromDateTimeOffset(clock.GetDateTime());
            AppendHistory(context, nameof(AddSegment), now);
            AggregateVersion++;
        }

        [SpecRef("CDR-2.1.9-RemoveTraveler")]
        public void RemoveTraveler(
            TravelerId travelerId,
            IAuthenticatedContext context,
            IIdGenerator idGenerator,
            IClock clock)
        {
            EnsureFlagsClear();
            EnsureServicingAuthority(context);

            var traveler = RequireTraveler(travelerId);

            if (_items.Any(item => item.TravelerRef == travelerId && item.IsActive))
                throw ExceptionFactory.CannotRemoveTravelerWithActiveItem(travelerId);

            _segments.RemoveAll(segment => segment.TravelerRef == travelerId);
            _travelers.Remove(traveler);

            var now = Instant.FromDateTimeOffset(clock.GetDateTime());
            AppendHistory(context, nameof(RemoveTraveler), now);
            AggregateVersion++;

            Causes(new TravelerRemoved(
                EventId: idGenerator.NewId().ToString(),
                AggregateId: Id.ToString(),
                TravelerId: travelerId,
                AggregateVersion: AggregateVersion,
                TimeOfOccurrence: clock.GetDateTime()));
        }

        [SpecRef("DD-ENT-003-05")]
        public void AssignInventoryHold(
            TravelerId travelerRef,
            JourneyElementId journeyRef,
            InventoryHoldRef hold,
            IAuthenticatedContext context,
            IClock clock)
        {
            EnsureFlagsClear();
            EnsureServicingAuthority(context);

            RequireSegment(travelerRef, journeyRef).AssignHold(hold);

            var now = Instant.FromDateTimeOffset(clock.GetDateTime());
            AppendHistory(context, nameof(AssignInventoryHold), now);
            AggregateVersion++;
        }

        [SpecRef("DD-ENT-003-06")]
        public void AssignSeat(
            TravelerId travelerRef,
            JourneyElementId journeyRef,
            SeatAssignment seat,
            IAuthenticatedContext context,
            IClock clock)
        {
            EnsureFlagsClear();
            EnsureServicingAuthority(context);

            var segment = RequireSegment(travelerRef, journeyRef);
            EnsureSegmentNotUnderExternalControl(segment);
            segment.AssignSeat(seat);

            var now = Instant.FromDateTimeOffset(clock.GetDateTime());
            AppendHistory(context, nameof(AssignSeat), now);
            AggregateVersion++;
        }

        [SpecRef("DD-AGG-001-10")]
        public void FlagForReview(bool requiresReview) => RequiresReview = requiresReview;

        [SpecRef("DD-AGG-001-09")]
        public void SetLegalHold(bool isUnderLegalHold) => IsUnderLegalHold = isUnderLegalHold;

        [SpecRef("DD-AGG-001-06")]
        private OrderTotals ComputeTotals()
        {
            var currency = SalesContext.SaleCurrency;

            var totalPrice = _items
                .Where(item => item.IsActive)
                .Aggregate(Money.Zero(currency), (running, item) => running + item.Price.Total);

            var totalSettled = _payments
                .Where(payment => payment.IsSettled)
                .SelectMany(payment => payment.Allocations)
                .Aggregate(Money.Zero(currency), (running, allocation) => running + allocation.Amount);

            return new OrderTotals(totalPrice, totalSettled, totalPrice - totalSettled);
        }

        [SpecRef("DD-VO-028")]
        private void AppendHistory(IAuthenticatedContext context, string commandName, Instant occurredAtUtc)
        {
            _history.Add(new OrderHistoryEntry(
                sequenceNo: _history.Count + 1,
                occurredAtUtc: occurredAtUtc,
                commandName: commandName,
                actor: context.Actor,
                channel: context.Channel,
                correlationId: context.CorrelationId,
                causationId: context.CausationId,
                resultingAggregateVersion: AggregateVersion + 1));
        }

        private void RecordContactChange(
            IAuthenticatedContext context,
            string commandName,
            IIdGenerator idGenerator,
            IClock clock)
        {
            var now = Instant.FromDateTimeOffset(clock.GetDateTime());
            AppendHistory(context, commandName, now);
            AggregateVersion++;

            Causes(new ContactChanged(
                EventId: idGenerator.NewId().ToString(),
                AggregateId: Id.ToString(),
                AggregateVersion: AggregateVersion,
                TimeOfOccurrence: clock.GetDateTime()));
        }

        private void RecordSuspensionChange(
            IAuthenticatedContext context,
            string commandName,
            IIdGenerator idGenerator,
            IClock clock)
        {
            var now = Instant.FromDateTimeOffset(clock.GetDateTime());
            AppendHistory(context, commandName, now);
            AggregateVersion++;

            Causes(new SuspensionChanged(
                EventId: idGenerator.NewId().ToString(),
                AggregateId: Id.ToString(),
                IsSuspended: IsSuspended,
                AggregateVersion: AggregateVersion,
                TimeOfOccurrence: clock.GetDateTime()));
        }

        [SpecRef("DD-AGG-001-08")]
        private void EnsureFlagsClear()
        {
            if (IsSuspended)
                throw ExceptionFactory.OrderIsSuspended();

            if (IsUnderLegalHold)
                throw ExceptionFactory.OrderIsUnderLegalHold();

            if (ClosedAtUtc is not null)
                throw ExceptionFactory.OrderIsClosed();
        }

        [SpecRef("DD-VO-017")]
        private void EnsureServicingAuthority(IAuthenticatedContext context)
        {
            if (context.Actor.HasAirlineOverride)
                return;

            if (context.SellerId != SalesContext.SellerId)
                throw ExceptionFactory.ServicingRequiresMatchingSellerOrOverride();
        }

        private void EnsureItemMayBeCancelled(
            OrderItem item,
            bool deliveryUnitsWithdrawn,
            bool valueReturnCompletedOrReserved)
        {
            foreach (var journeyRef in item.JourneyRefs)
            {
                var segment = _segments.FirstOrDefault(segment =>
                    segment.TravelerRef == item.TravelerRef && segment.JourneyRef == journeyRef);

                if (segment is null)
                    continue;

                if (segment.IsConsumed)
                    throw ExceptionFactory.CannotCancelFlownItem(item.Id);

                EnsureSegmentNotUnderExternalControl(segment);
            }

            if (item.IsDeliveredOrBeyond && !deliveryUnitsWithdrawn)
                throw ExceptionFactory.DeliveredCancellationRequiresVoidedUnits(item.Id);

            if (IsItemSettled(item) && !valueReturnCompletedOrReserved)
                throw ExceptionFactory.DeliveredCancellationRequiresVoidedUnits(item.Id);
        }

        [SpecRef("DD-ENT-003-04")]
        private void EnsureSegmentNotUnderExternalControl(TravelerJourneySegment segment)
        {
            if (segment.IsUnderExternalControl)
                throw ExceptionFactory.RejectedWhileUnderExternalControl(segment.TravelerRef, segment.JourneyRef);
        }

        private void EnsureInfantsHaveAssociatedAdult()
        {
            foreach (var traveler in _travelers.Where(traveler => traveler.Ptc == PassengerTypeCode.INF))
            {
                if (traveler.AssociatedAdult is null ||
                    _travelers.All(candidate => candidate.Id != traveler.AssociatedAdult.Value))
                    throw ExceptionFactory.InfantRequiresAssociatedAdult(traveler.Id);
            }
        }

        private void EnsureAssociatedAdultIsValid(
            PassengerTypeCode ptc,
            TravelerId travelerId,
            TravelerId? associatedAdult)
        {
            if (ptc != PassengerTypeCode.INF)
                return;

            if (associatedAdult is null)
                throw ExceptionFactory.InfantRequiresAssociatedAdult(travelerId);

            var adult = _travelers.FirstOrDefault(traveler => traveler.Id == associatedAdult.Value);

            if (adult is not null && adult.Ptc != PassengerTypeCode.ADT)
                throw ExceptionFactory.AssociatedAdultMustBeAdult(travelerId);
        }

        private void EnsureFlightItemsHaveSegmentPerJourney()
        {
            foreach (var item in _items.Where(item => item.Type == ItemType.Flight && item.IsActive))
            {
                foreach (var journeyRef in item.JourneyRefs)
                {
                    if (!_segments.Any(segment =>
                            segment.TravelerRef == item.TravelerRef && segment.JourneyRef == journeyRef))
                        throw ExceptionFactory.FlightItemRequiresSegmentPerJourney(item.Id, item.TravelerRef);
                }
            }
        }

        private void EnsureAncillaryHasConfirmedFlightItem(
            TravelerId travelerRef,
            IReadOnlyList<JourneyElementId> journeyRefs)
        {
            var journeyRef = journeyRefs[0];

            var hasConfirmedFlight = _items.Any(item =>
                item.Type == ItemType.Flight &&
                item.TravelerRef == travelerRef &&
                item.JourneyRefs.Contains(journeyRef) &&
                item.IsConfirmedOrBeyond);

            if (!hasConfirmedFlight)
                throw ExceptionFactory.AncillaryRequiresConfirmedFlightItem(journeyRef);
        }

        private void CancelSegmentsWithoutActiveItems()
        {
            foreach (var segment in _segments)
            {
                if (segment.SegmentStatus == SegmentStatus.Cancelled)
                    continue;

                var hasActiveItem = _items.Any(item =>
                    item.IsActive &&
                    item.TravelerRef == segment.TravelerRef &&
                    item.JourneyRefs.Contains(segment.JourneyRef));

                if (!hasActiveItem)
                    segment.TransitionSegmentStatus(SegmentStatus.Cancelled);
            }
        }

        private void MarkItemsConsumedFor(TravelerId travelerRef, JourneyElementId journeyRef)
        {
            foreach (var item in _items.Where(item =>
                         item.TravelerRef == travelerRef &&
                         item.JourneyRefs.Contains(journeyRef) &&
                         item.IsDeliveredOrBeyond))
            {
                var allConsumed = item.JourneyRefs.All(reference =>
                    _segments.Any(segment =>
                        segment.TravelerRef == travelerRef &&
                        segment.JourneyRef == reference &&
                        segment.IsConsumed));

                if (allConsumed)
                {
                    if (item.Status != OrderItemStatus.Used)
                        item.MarkUsed();
                }
                else if (item.Status == OrderItemStatus.Delivered)
                {
                    item.MarkPartiallyUsed();
                }
            }
        }

        private void MarkCoveringTimeLimitsMet()
        {
            foreach (var timeLimit in _timeLimits.Where(timeLimit => timeLimit.IsActive))
            {
                var allSettled = timeLimit.AppliesTo.All(itemRef =>
                {
                    var item = _items.FirstOrDefault(item => item.Id == itemRef);
                    return item is null || item.Status != OrderItemStatus.PendingPayment;
                });

                if (allSettled)
                    timeLimit.MarkMet();
            }
        }

        [SpecRef("DD-ENT-004-12")]
        private bool IsItemBalanceSettled(OrderItem item)
        {
            if (item.CreditAuthority is not null)
                return true;

            var allocated = _payments
                .Where(payment => payment.IsSettled)
                .Aggregate(
                    Money.Zero(SalesContext.SaleCurrency),
                    (running, payment) => running + payment.AllocatedTo(item.Id));

            return allocated >= item.Price.Total;
        }

        private bool IsItemSettled(OrderItem item) =>
            _payments
                .Where(payment => payment.IsSettled)
                .Any(payment => !payment.AllocatedTo(item.Id).IsZero);

        private Traveler RequireTraveler(TravelerId travelerId) =>
            _travelers.FirstOrDefault(traveler => traveler.Id == travelerId)
            ?? throw ExceptionFactory.TravelerNotFound(travelerId);

        private JourneyElement RequireJourneyElement(JourneyElementId journeyElementId) =>
            _journeyElements.FirstOrDefault(element => element.Id == journeyElementId)
            ?? throw ExceptionFactory.JourneyElementNotFound(journeyElementId);

        private OrderItem RequireItem(OrderItemId orderItemId) =>
            _items.FirstOrDefault(item => item.Id == orderItemId)
            ?? throw ExceptionFactory.OrderItemNotFound(orderItemId);

        private PaymentRecord RequirePayment(PaymentRecordId paymentRecordId) =>
            _payments.FirstOrDefault(payment => payment.Id == paymentRecordId)
            ?? throw ExceptionFactory.PaymentRecordNotFound(paymentRecordId);

        private TimeLimit RequireTimeLimit(TimeLimitId timeLimitId) =>
            _timeLimits.FirstOrDefault(timeLimit => timeLimit.Id == timeLimitId)
            ?? throw ExceptionFactory.TimeLimitNotFound(timeLimitId);

        private TravelerJourneySegment RequireSegment(TravelerId travelerRef, JourneyElementId journeyRef) =>
            _segments.FirstOrDefault(segment =>
                segment.TravelerRef == travelerRef && segment.JourneyRef == journeyRef)
            ?? throw ExceptionFactory.SegmentNotFound(travelerRef, journeyRef);

        private IEnumerable<TravelerJourneySegment> SegmentsOn(JourneyElementId journeyRef) =>
            _segments.Where(segment =>
                segment.JourneyRef == journeyRef && segment.SegmentStatus != SegmentStatus.Cancelled);
    }
}
