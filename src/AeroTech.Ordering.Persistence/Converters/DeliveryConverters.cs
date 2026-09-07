using AeroTech.Ordering.Domain._Shared.Identifiers;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AeroTech.Ordering.Persistence.Converters
{
    public sealed class GroupBookingIdConverter : ValueConverter<GroupBookingId, Guid>
    {
        public GroupBookingIdConverter() : base(id => id.Value, value => new GroupBookingId(value))
        {
        }
    }

    public sealed class NullableGroupBookingIdConverter : ValueConverter<GroupBookingId?, Guid?>
    {
        public NullableGroupBookingIdConverter()
            : base(
                id => id == null ? null : id.Value.Value,
                value => value == null ? null : new GroupBookingId(value.Value))
        {
        }
    }

    public sealed class RangeIdConverter : ValueConverter<RangeId, Guid>
    {
        public RangeIdConverter() : base(id => id.Value, value => new RangeId(value))
        {
        }
    }

    public sealed class BranchIdConverter : ValueConverter<BranchId, string>
    {
        public BranchIdConverter() : base(id => id.Value, value => new BranchId(value))
        {
        }
    }

    public sealed class NullableBranchIdConverter : ValueConverter<BranchId?, string?>
    {
        public NullableBranchIdConverter()
            : base(
                id => id == null ? null : id.Value.Value,
                value => value == null ? null : new BranchId(value))
        {
        }
    }

    public sealed class BlockIdConverter : ValueConverter<BlockId, string>
    {
        public BlockIdConverter() : base(id => id.Value, value => new BlockId(value))
        {
        }
    }

    public sealed class SlotIdConverter : ValueConverter<SlotId, string>
    {
        public SlotIdConverter() : base(id => id.Value, value => new SlotId(value))
        {
        }
    }

    public sealed class GroupReferenceConverter : ValueConverter<GroupReference, string>
    {
        public GroupReferenceConverter()
            : base(reference => reference.Value, value => new GroupReference(value))
        {
        }
    }

    public sealed class DeliveryRecordNumberConverter : ValueConverter<DeliveryRecordNumber, string>
    {
        public DeliveryRecordNumberConverter()
            : base(number => number.Value, value => new DeliveryRecordNumber(value))
        {
        }
    }

    public sealed class NullableDeliveryRecordNumberConverter
        : ValueConverter<DeliveryRecordNumber?, string?>
    {
        public NullableDeliveryRecordNumberConverter()
            : base(
                number => number == null ? null : number.Value.Value,
                value => value == null ? null : new DeliveryRecordNumber(value))
        {
        }
    }

    public sealed class DocumentNumberConverter : ValueConverter<DocumentNumber, string>
    {
        public DocumentNumberConverter()
            : base(number => number.Value, value => new DocumentNumber(value))
        {
        }
    }

    public sealed class NullableDocumentNumberConverter : ValueConverter<DocumentNumber?, string?>
    {
        public NullableDocumentNumberConverter()
            : base(
                number => number == null ? null : number.Value.Value,
                value => value == null ? null : new DocumentNumber(value))
        {
        }
    }

    public sealed class NullableOrderIdConverter : ValueConverter<OrderId?, Guid?>
    {
        public NullableOrderIdConverter()
            : base(
                id => id == null ? null : id.Value.Value,
                value => value == null ? null : new OrderId(value.Value))
        {
        }
    }
}
