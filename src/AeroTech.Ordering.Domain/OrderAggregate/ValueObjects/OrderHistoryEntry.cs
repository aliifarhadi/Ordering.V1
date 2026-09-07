using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Enums;
using AeroTech.Ordering.Domain._Shared.Resources;
using AeroTech.Ordering.Domain._Shared.ValueObjects;
using NodaTime;

namespace AeroTech.Ordering.Domain.OrderAggregate.ValueObjects
{
    [SpecRef("DD-VO-028")]
    [Blocked("BL-008", "Snapshot content and reconstruction strategy", "ADR-14")]
    public sealed record OrderHistoryEntry
    {
#pragma warning disable CS8618
        private OrderHistoryEntry()
        {
        }
#pragma warning restore CS8618

        [SpecRef("DD-VO-028")]
        public OrderHistoryEntry(
            long sequenceNo,
            Instant occurredAtUtc,
            string commandName,
            ActorRef actor,
            ChannelCode channel,
            string correlationId,
            string? causationId,
            long resultingAggregateVersion)
        {
            if (string.IsNullOrWhiteSpace(commandName))
                throw ExceptionFactory.IdentifierIsRequired(nameof(commandName));

            if (string.IsNullOrWhiteSpace(correlationId))
                throw ExceptionFactory.IdentifierIsRequired(nameof(correlationId));

            SequenceNo = sequenceNo;
            OccurredAtUtc = occurredAtUtc;
            CommandName = commandName;
            Actor = actor;
            Channel = channel;
            CorrelationId = correlationId;
            CausationId = causationId;
            ResultingAggregateVersion = resultingAggregateVersion;
        }

        [SpecRef("DD-VO-028")]
        public long SequenceNo { get; }

        [SpecRef("DD-VO-028")]
        public Instant OccurredAtUtc { get; }

        [SpecRef("DD-VO-028")]
        public string CommandName { get; }

        [SpecRef("DD-VO-028")]
        public ActorRef Actor { get; }

        [SpecRef("DD-VO-028")]
        public ChannelCode Channel { get; }

        [SpecRef("DD-VO-028")]
        public string CorrelationId { get; }

        [SpecRef("DD-VO-028")]
        public string? CausationId { get; }

        [SpecRef("DD-VO-028")]
        public long ResultingAggregateVersion { get; }
    }
}
