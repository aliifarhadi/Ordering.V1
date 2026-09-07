using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Enums;
using AeroTech.Ordering.Domain._Shared.Identifiers;
using AeroTech.Ordering.Domain._Shared.Resources;
using AeroTech.Ordering.Domain._Shared.ValueObjects;

namespace AeroTech.Ordering.Domain.DeliveryRecordAggregate.Entities
{
    [SpecRef("CDR-3.1.3")]
    public sealed class ServiceDelivery
    {
        private ServiceDelivery()
        {
        }

        [SpecRef("CDR-3.1.3")]
        internal ServiceDelivery(
            int unitNumber,
            OrderItemId orderItemRef,
            string serviceCode,
            string description,
            Money unitValue,
            int? associatedSegmentUnit)
        {
            if (string.IsNullOrWhiteSpace(serviceCode))
                throw ExceptionFactory.IdentifierIsRequired(nameof(serviceCode));

            UnitNumber = unitNumber;
            OrderItemRef = orderItemRef;
            ServiceCode = serviceCode;
            Description = description;
            UnitValue = unitValue;
            AssociatedSegmentUnit = associatedSegmentUnit;
            Status = ServiceDeliveryStatus.Open;
        }

        [SpecRef("CDR-3.1.3")]
        public int UnitNumber { get; private set; }

        [SpecRef("CDR-3.1.3")]
        public OrderItemId OrderItemRef { get; private set; }

        [SpecRef("CDR-3.1.3")]
        public string ServiceCode { get; private set; } = null!;

        [SpecRef("CDR-3.1.3")]
        public string Description { get; private set; } = null!;

        [SpecRef("CDR-3.1.3")]
        public ServiceDeliveryStatus Status { get; private set; }

        [SpecRef("CDR-3.1.3")]
        public Money UnitValue { get; private set; } = null!;

        [SpecRef("CDR-3.1.3")]
        public int? AssociatedSegmentUnit { get; private set; }

        [SpecRef("CDR-3.1.7")]
        public bool IsOpen => Status == ServiceDeliveryStatus.Open;

        [SpecRef("CDR-3.1.7")]
        public bool IsTerminal => Status is
            ServiceDeliveryStatus.Refunded or
            ServiceDeliveryStatus.Void;

        [SpecRef("CDR-3.1.6")]
        internal void Consume() => TransitionTo(ServiceDeliveryStatus.Consumed);

        [SpecRef("CDR-3.1.4-VoidRecord")]
        internal void Void() => TransitionTo(ServiceDeliveryStatus.Void);

        [SpecRef("CDR-3.1.4-RefundUnits")]
        internal void Refund() => TransitionTo(ServiceDeliveryStatus.Refunded);

        private void TransitionTo(ServiceDeliveryStatus target)
        {
            if (!IsTransitionPermitted(Status, target))
                throw ExceptionFactory.ServiceDeliveryCannotTransition(UnitNumber, Status, target);

            Status = target;
        }

        [SpecRef("CDR-3.1.6")]
        private static bool IsTransitionPermitted(ServiceDeliveryStatus from, ServiceDeliveryStatus to) =>
            (from, to) switch
            {
                (ServiceDeliveryStatus.Open, ServiceDeliveryStatus.Consumed) => true,
                (ServiceDeliveryStatus.Open, ServiceDeliveryStatus.Void) => true,
                (ServiceDeliveryStatus.Open, ServiceDeliveryStatus.Refunded) => true,
                (ServiceDeliveryStatus.Consumed, ServiceDeliveryStatus.Refunded) => true,
                _ => false,
            };
    }
}
