using AeroTech.Ordering.Domain._Shared.Identifiers;
using AeroTech.Ordering.Domain._Shared.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AeroTech.Ordering.Persistence.Converters
{
    public sealed class OrderIdConverter : ValueConverter<OrderId, Guid>
    {
        public OrderIdConverter() : base(id => id.Value, value => new OrderId(value))
        {
        }
    }

    public sealed class OperationIdConverter : ValueConverter<OperationId, Guid>
    {
        public OperationIdConverter() : base(id => id.Value, value => new OperationId(value))
        {
        }
    }

    public sealed class TravelerIdConverter : ValueConverter<TravelerId, string>
    {
        public TravelerIdConverter() : base(id => id.Value, value => new TravelerId(value))
        {
        }
    }

    public sealed class NullableTravelerIdConverter : ValueConverter<TravelerId?, string?>
    {
        public NullableTravelerIdConverter()
            : base(
                id => id == null ? null : id.Value.Value,
                value => value == null ? null : new TravelerId(value))
        {
        }
    }

    public sealed class JourneyElementIdConverter : ValueConverter<JourneyElementId, string>
    {
        public JourneyElementIdConverter() : base(id => id.Value, value => new JourneyElementId(value))
        {
        }
    }

    public sealed class OrderItemIdConverter : ValueConverter<OrderItemId, string>
    {
        public OrderItemIdConverter() : base(id => id.Value, value => new OrderItemId(value))
        {
        }
    }

    public sealed class NullableOrderItemIdConverter : ValueConverter<OrderItemId?, string?>
    {
        public NullableOrderItemIdConverter()
            : base(
                id => id == null ? null : id.Value.Value,
                value => value == null ? null : new OrderItemId(value))
        {
        }
    }

    public sealed class PaymentRecordIdConverter : ValueConverter<PaymentRecordId, string>
    {
        public PaymentRecordIdConverter() : base(id => id.Value, value => new PaymentRecordId(value))
        {
        }
    }

    public sealed class TimeLimitIdConverter : ValueConverter<TimeLimitId, string>
    {
        public TimeLimitIdConverter() : base(id => id.Value, value => new TimeLimitId(value))
        {
        }
    }

    public sealed class CustomerIdConverter : ValueConverter<CustomerId?, string?>
    {
        public CustomerIdConverter()
            : base(
                id => id == null ? null : id.Value.Value,
                value => value == null ? null : new CustomerId(value))
        {
        }
    }

    public sealed class SellerIdConverter : ValueConverter<SellerId, string>
    {
        public SellerIdConverter() : base(id => id.Value, value => new SellerId(value))
        {
        }
    }

    public sealed class NullableSellerIdConverter : ValueConverter<SellerId?, string?>
    {
        public NullableSellerIdConverter()
            : base(
                id => id == null ? null : id.Value.Value,
                value => value == null ? null : new SellerId(value))
        {
        }
    }

    public sealed class OfferIdConverter : ValueConverter<OfferId, string>
    {
        public OfferIdConverter() : base(id => id.Value, value => new OfferId(value))
        {
        }
    }

    public sealed class MarriedGroupIdConverter : ValueConverter<MarriedGroupId?, string?>
    {
        public MarriedGroupIdConverter()
            : base(
                id => id == null ? null : id.Value.Value,
                value => value == null ? null : new MarriedGroupId(value))
        {
        }
    }

    public sealed class OrderReferenceConverter : ValueConverter<OrderReference, string>
    {
        public OrderReferenceConverter() : base(reference => reference.Value, value => new OrderReference(value))
        {
        }
    }

    public sealed class CurrencyCodeConverter : ValueConverter<CurrencyCode, string>
    {
        public CurrencyCodeConverter() : base(currency => currency.Value, value => new CurrencyCode(value))
        {
        }
    }

    public sealed class CarrierCodeConverter : ValueConverter<CarrierCode, string>
    {
        public CarrierCodeConverter() : base(code => code.Value, value => new CarrierCode(value))
        {
        }
    }

    public sealed class FlightNumberConverter : ValueConverter<FlightNumber, string>
    {
        public FlightNumberConverter() : base(number => number.Value, value => new FlightNumber(value))
        {
        }
    }

    public sealed class AirportCodeConverter : ValueConverter<AirportCode, string>
    {
        public AirportCodeConverter() : base(code => code.Value, value => new AirportCode(value))
        {
        }
    }

    public sealed class CabinCodeConverter : ValueConverter<CabinCode, string>
    {
        public CabinCodeConverter() : base(code => code.Value, value => new CabinCode(value))
        {
        }
    }

    public sealed class RbdCodeConverter : ValueConverter<RbdCode, string>
    {
        public RbdCodeConverter() : base(code => code.Value, value => new RbdCode(value))
        {
        }
    }

    public sealed class InventoryHoldRefConverter : ValueConverter<InventoryHoldRef?, string?>
    {
        public InventoryHoldRefConverter()
            : base(
                reference => reference == null ? null : reference.Value.Value,
                value => value == null ? null : new InventoryHoldRef(value))
        {
        }
    }

    public sealed class PaymentRequestRefConverter : ValueConverter<PaymentRequestRef?, string?>
    {
        public PaymentRequestRefConverter()
            : base(
                reference => reference == null ? null : reference.Value.Value,
                value => value == null ? null : new PaymentRequestRef(value))
        {
        }
    }
}
