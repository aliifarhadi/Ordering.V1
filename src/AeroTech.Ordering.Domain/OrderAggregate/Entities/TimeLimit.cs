using AeroTech.Framework.Core.Domain.Entities;
using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Contracts;
using AeroTech.Ordering.Domain._Shared.Enums;
using AeroTech.Ordering.Domain._Shared.Identifiers;
using AeroTech.Ordering.Domain._Shared.Resources;
using NodaTime;

namespace AeroTech.Ordering.Domain.OrderAggregate.Entities
{
    [SpecRef("DD-ENT-006")]
    public sealed class TimeLimit : Entity<TimeLimitId>
    {
        private readonly List<OrderItemId> _appliesTo = [];

        private TimeLimit()
        {
        }

        [SpecRef("DD-ENT-006")]
        internal TimeLimit(
            TimeLimitId id,
            TimeLimitType type,
            Instant dueAtUtc,
            IReadOnlyList<OrderItemId> appliesTo,
            Instant now)
        {
            if (dueAtUtc <= now)
                throw ExceptionFactory.TimeLimitMustBeInTheFuture();

            Id = id;
            Type = type;
            DueAtUtc = dueAtUtc;
            Status = TimeLimitStatus.Active;
            ExtensionCount = 0;
            _appliesTo = [.. appliesTo];
        }

        [SpecRef("DD-ENT-006")]
        public TimeLimitType Type { get; private set; }

        [SpecRef("DD-ENT-006")]
        public Instant DueAtUtc { get; private set; }

        [SpecRef("DD-ENT-006")]
        public TimeLimitStatus Status { get; private set; }

        [SpecRef("DD-ENT-006")]
        public IReadOnlyList<OrderItemId> AppliesTo => _appliesTo.AsReadOnly();

        [SpecRef("DD-ENT-006")]
        public int ExtensionCount { get; private set; }

        [SpecRef("DD-ENT-006")]
        public bool IsActive => Status == TimeLimitStatus.Active;

        [SpecRef("DD-CMD-013")]
        internal void Extend(Instant dueAtUtc, IDomainLimits limits, Instant now)
        {
            if (!IsActive)
                throw ExceptionFactory.TimeLimitNotActive(Id);

            if (ExtensionCount >= limits.MaxTimeLimitExtensions)
                throw ExceptionFactory.TimeLimitExtensionsExhausted(Id);

            if (dueAtUtc <= now)
                throw ExceptionFactory.TimeLimitMustBeInTheFuture();

            if (dueAtUtc <= DueAtUtc)
                throw ExceptionFactory.TimeLimitExtensionMustBeLater();

            DueAtUtc = dueAtUtc;
            ExtensionCount++;
        }

        [SpecRef("DD-CMD-014")]
        internal void MarkExpired()
        {
            if (!IsActive)
                throw ExceptionFactory.TimeLimitNotActive(Id);

            Status = TimeLimitStatus.Expired;
        }

        [SpecRef("DD-ENT-006")]
        internal void MarkMet()
        {
            if (!IsActive)
                throw ExceptionFactory.TimeLimitNotActive(Id);

            Status = TimeLimitStatus.Met;
        }

        [SpecRef("DD-ENT-006")]
        internal void MarkCancelled()
        {
            if (!IsActive)
                throw ExceptionFactory.TimeLimitNotActive(Id);

            Status = TimeLimitStatus.Cancelled;
        }
    }
}
