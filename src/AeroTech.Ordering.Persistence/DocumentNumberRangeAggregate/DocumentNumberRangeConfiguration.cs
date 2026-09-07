using AeroTech.Ordering.Domain.DocumentNumberRangeAggregate;
using AeroTech.Ordering.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroTech.Ordering.Persistence.DocumentNumberRangeAggregate
{
    public sealed class DocumentNumberRangeConfiguration : IEntityTypeConfiguration<DocumentNumberRange>
    {
        public void Configure(EntityTypeBuilder<DocumentNumberRange> builder)
        {
            builder.ToTable("DocumentNumberRanges", OrderingDbContext.DeliverySchema);

            builder.HasKey(range => range.Id);

            builder.Property(range => range.Id)
                .HasConversion<RangeIdConverter>()
                .HasColumnName("RangeId")
                .ValueGeneratedNever();

            builder.Property(range => range.BranchRef)
                .HasConversion<BranchIdConverter>()
                .HasColumnName("BranchId").HasMaxLength(64).IsRequired();

            builder.Property(range => range.DocumentKind)
                .HasConversion<string>().HasMaxLength(32).IsRequired();

            builder.Property(range => range.Prefix).HasMaxLength(16);
            builder.Property(range => range.RangeStart).IsRequired();
            builder.Property(range => range.RangeEnd).IsRequired();
            builder.Property(range => range.NextAvailable).IsRequired();

            builder.Property(range => range.Status)
                .HasConversion<string>().HasMaxLength(32).IsRequired();

            builder.Property(range => range.RegisteredWithAuthorityAtUtc)
                .HasConversion<NullableInstantConverter>().HasColumnType("datetime2(7)");

            builder.Property(range => range.AuthorityReference).HasMaxLength(128);

            builder.Property(range => range.RowVersion).IsRowVersion();

            builder.Ignore(range => range.IsExhausted);

            builder.HasIndex(range => new { range.BranchRef, range.DocumentKind, range.Status })
                .HasFilter("[Status] = 'Active'")
                .IsUnique();
        }
    }
}
