using AeroTech.Ordering.Domain._Shared.Identifiers;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AeroTech.Ordering.Persistence.Converters
{
    public sealed class JourneyElementIdListConverter
        : ValueConverter<IReadOnlyList<JourneyElementId>, string>
    {
        public JourneyElementIdListConverter()
            : base(
                refs => string.Join(',', refs.Select(reference => reference.Value)),
                value => value.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(part => new JourneyElementId(part))
                    .ToList())
        {
        }
    }

    public sealed class JourneyElementIdListComparer
        : ValueComparer<IReadOnlyList<JourneyElementId>>
    {
        public JourneyElementIdListComparer()
            : base(
                (left, right) => left!.SequenceEqual(right!),
                refs => refs.Aggregate(0, (hash, reference) => HashCode.Combine(hash, reference.GetHashCode())),
                refs => refs.ToList())
        {
        }
    }

    public sealed class OrderIdListConverter
        : ValueConverter<IReadOnlyList<OrderId>, string>
    {
        public OrderIdListConverter()
            : base(
                refs => string.Join(',', refs.Select(reference => reference.Value.ToString())),
                value => value.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(part => new OrderId(Guid.Parse(part)))
                    .ToList())
        {
        }
    }

    public sealed class OrderIdListComparer
        : ValueComparer<IReadOnlyList<OrderId>>
    {
        public OrderIdListComparer()
            : base(
                (left, right) => left!.SequenceEqual(right!),
                refs => refs.Aggregate(0, (hash, reference) => HashCode.Combine(hash, reference.GetHashCode())),
                refs => refs.ToList())
        {
        }
    }

    public sealed class OrderItemIdListConverter
        : ValueConverter<IReadOnlyList<OrderItemId>, string>
    {
        public OrderItemIdListConverter()
            : base(
                refs => string.Join(',', refs.Select(reference => reference.Value)),
                value => value.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(part => new OrderItemId(part))
                    .ToList())
        {
        }
    }

    public sealed class OrderItemIdListComparer
        : ValueComparer<IReadOnlyList<OrderItemId>>
    {
        public OrderItemIdListComparer()
            : base(
                (left, right) => left!.SequenceEqual(right!),
                refs => refs.Aggregate(0, (hash, reference) => HashCode.Combine(hash, reference.GetHashCode())),
                refs => refs.ToList())
        {
        }
    }
}
