using AeroTech.Ordering.Domain.OperationAggregate;
using AeroTech.Ordering.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AeroTech.Ordering.Persistence.OperationAggregate
{
    public sealed class OperationConfiguration : IEntityTypeConfiguration<Operation>
    {
        public void Configure(EntityTypeBuilder<Operation> builder)
        {
            builder.ToTable("Operations", OrderingDbContext.OperationsSchema);

            builder.HasKey(operation => operation.Id);

            builder.Property(operation => operation.Id)
                .HasConversion<OperationIdConverter>()
                .ValueGeneratedNever();

            builder.Property(operation => operation.Type)
                .HasConversion<string>().HasMaxLength(32).IsRequired();

            builder.Property(operation => operation.OrderRef)
                .HasConversion<NullableOrderIdConverter>()
                .HasColumnName("OrderId");

            builder.Property(operation => operation.GroupRef)
                .HasConversion<NullableGroupBookingIdConverter>()
                .HasColumnName("GroupBookingId");

            builder.Property(operation => operation.Status)
                .HasConversion<string>().HasMaxLength(32).IsRequired();

            builder.Property(operation => operation.CurrentStep).HasMaxLength(128).IsRequired();
            builder.Property(operation => operation.ExpectedExternalMessage).HasMaxLength(128);
            builder.Property(operation => operation.CorrelationId).HasMaxLength(128).IsRequired();
            builder.Property(operation => operation.CausationId).HasMaxLength(128);
            builder.Property(operation => operation.AttemptCount).IsRequired();

            builder.Property(operation => operation.NextRetryAtUtc)
                .HasConversion<NullableInstantConverter>().HasColumnType("datetime2(7)");

            builder.Property(operation => operation.TimeoutAtUtc)
                .HasConversion<NullableInstantConverter>().HasColumnType("datetime2(7)");

            builder.Property(operation => operation.CompensationPolicy)
                .HasConversion<string>().HasMaxLength(32).IsRequired();

            builder.Property(operation => operation.Result).HasColumnType("nvarchar(max)");
            builder.Property(operation => operation.FailureReason).HasMaxLength(1024);

            builder.Property(operation => operation.IdempotencyKey).HasMaxLength(128).IsRequired();

            builder.Property(operation => operation.RowVersion).IsRowVersion();

            builder.Ignore(operation => operation.IsOpen);

            builder.HasIndex(operation => new { operation.Status, operation.NextRetryAtUtc });
            builder.HasIndex(operation => new { operation.Status, operation.TimeoutAtUtc });
            builder.HasIndex(operation => new { operation.OrderRef, operation.Type, operation.Status });
            builder.HasIndex(operation => operation.IdempotencyKey).IsUnique();

            builder.Property(operation => operation.ScopedItems)
                .HasConversion(
                    new OrderItemIdListConverter(),
                    new OrderItemIdListComparer())
                .HasColumnName("ScopedItems")
                .HasMaxLength(512);

            builder.OwnsMany(operation => operation.StepHistory, step =>
            {
                step.ToTable("OperationSteps", OrderingDbContext.OperationsSchema);
                step.WithOwner().HasForeignKey("OperationId");
                step.Property<int>("Seq").ValueGeneratedOnAdd();
                step.HasKey("OperationId", "Seq");

                step.Property(value => value.Name).HasMaxLength(128).IsRequired();

                step.Property(value => value.EnteredAtUtc)
                    .HasConversion<InstantConverter>()
                    .HasColumnType("datetime2(7)").IsRequired();

                step.Property(value => value.CompletedAtUtc)
                    .HasConversion<NullableInstantConverter>()
                    .HasColumnType("datetime2(7)");

                step.Property(value => value.Outcome).HasMaxLength(1024);

                step.Ignore(value => value.IsOpen);
            });
        }
    }
}
