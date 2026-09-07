using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Enums;
using AeroTech.Ordering.Domain.DeliveryRecordAggregate.Entities;

namespace AeroTech.Ordering.Domain.DeliveryRecordAggregate.Specifications
{
    [SpecRef("CDR-3.1.7")]
    public static class DeliveryRecordStatusSpecification
    {
        [SpecRef("CDR-3.1.7")]
        public static DeliveryRecordStatus Derive(
            IReadOnlyList<SegmentDelivery> segmentUnits,
            IReadOnlyList<ServiceDelivery> serviceUnits)
        {
            var hasAnyUnit = segmentUnits.Count > 0 || serviceUnits.Count > 0;

            if (!hasAnyUnit)
                return DeliveryRecordStatus.Issued;

            if (segmentUnits.All(unit => unit.Status == SegmentDeliveryStatus.Void) &&
                serviceUnits.All(unit => unit.Status == ServiceDeliveryStatus.Void))
                return DeliveryRecordStatus.Void;

            if (segmentUnits.All(unit => unit.Status == SegmentDeliveryStatus.Refunded) &&
                serviceUnits.All(unit => unit.Status == ServiceDeliveryStatus.Refunded))
                return DeliveryRecordStatus.Refunded;

            if (segmentUnits.Count > 0 &&
                segmentUnits.All(unit => unit.Status == SegmentDeliveryStatus.Exchanged) &&
                serviceUnits.All(unit => unit.IsTerminal))
                return DeliveryRecordStatus.Exchanged;

            var allTerminal =
                segmentUnits.All(unit => unit.IsTerminal || unit.IsConsumed) &&
                serviceUnits.All(unit => unit.IsTerminal || unit.Status == ServiceDeliveryStatus.Consumed);

            var allSegmentsConsumed = segmentUnits.Count > 0 && segmentUnits.All(unit => unit.IsConsumed);
            var noOpenServiceUnit = serviceUnits.All(unit => !unit.IsOpen);

            if (allSegmentsConsumed && noOpenServiceUnit)
                return DeliveryRecordStatus.Used;

            if (allTerminal)
                return DeliveryRecordStatus.Closed;

            var anyProgressed =
                segmentUnits.Any(unit => unit.IsConsumed || unit.IsTerminal) ||
                serviceUnits.Any(unit => unit.IsTerminal || unit.Status == ServiceDeliveryStatus.Consumed);

            var anyOpen =
                segmentUnits.Any(unit => unit.Status == SegmentDeliveryStatus.Open || unit.IsUnderExternalControl) ||
                serviceUnits.Any(unit => unit.IsOpen);

            if (anyProgressed && anyOpen)
                return DeliveryRecordStatus.PartiallyUsed;

            return DeliveryRecordStatus.Issued;
        }
    }
}
