using AeroTech.Ordering.Domain.TaxDocumentAggregate;
using AeroTech.Ordering.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroTech.Ordering.Persistence.TaxDocumentAggregate
{
    public sealed class TaxDocumentConfiguration : IEntityTypeConfiguration<TaxDocument>
    {
        public void Configure(EntityTypeBuilder<TaxDocument> builder)
        {
            builder.ToTable("TaxDocuments", OrderingDbContext.DeliverySchema);

            builder.HasKey(document => document.Id);

            builder.Property(document => document.Id)
                .HasConversion<DocumentNumberConverter>()
                .HasColumnName("DocumentNumber")
                .HasMaxLength(64)
                .ValueGeneratedNever();

            builder.Property(document => document.Kind)
                .HasConversion<string>().HasMaxLength(32).IsRequired();

            builder.Property(document => document.OrderRef)
                .HasConversion<OrderIdConverter>()
                .HasColumnName("OrderId").IsRequired();

            builder.Property(document => document.DeliveryRecordRef)
                .HasConversion<NullableDeliveryRecordNumberConverter>()
                .HasColumnName("DeliveryRecordNumber").HasMaxLength(64);

            builder.Property(document => document.ReversesDocument)
                .HasConversion<NullableDocumentNumberConverter>()
                .HasColumnName("ReversesDocumentNumber").HasMaxLength(64);

            builder.Property(document => document.BranchRef)
                .HasConversion<BranchIdConverter>()
                .HasColumnName("BranchId").HasMaxLength(64).IsRequired();

            builder.Property(document => document.SpecVersion).HasMaxLength(32).IsRequired();
            builder.Property(document => document.BuyerTaxIdentity).HasMaxLength(128);
            builder.Property(document => document.PassengerIdentity).HasMaxLength(128).IsRequired();

            builder.Property(document => document.IssuedAtUtc)
                .HasConversion<InstantConverter>()
                .HasColumnType("datetime2(7)").IsRequired();

            builder.Property(document => document.SubmissionStatus)
                .HasConversion<string>().HasMaxLength(32).IsRequired();

            builder.Property(document => document.RowVersion).IsRowVersion();

            builder.Ignore(document => document.Number);

            builder.HasIndex(document => document.OrderRef);

            builder.HasIndex(document => new { document.SubmissionStatus, document.IssuedAtUtc })
                .HasFilter("[SubmissionStatus] = 'PendingSubmission'");

            builder.OwnsOne(document => document.TotalAmount, money =>
            {
                money.Property(amount => amount.Amount)
                    .HasColumnName("TotalAmount").HasColumnType("decimal(19,4)").IsRequired();
                money.Property(amount => amount.Currency)
                    .HasConversion<CurrencyCodeConverter>()
                    .HasColumnName("Currency").HasMaxLength(3).IsRequired();
            });

            builder.OwnsOne(document => document.TotalTax, money =>
            {
                money.Property(amount => amount.Amount)
                    .HasColumnName("TotalTax").HasColumnType("decimal(19,4)").IsRequired();
                money.Property(amount => amount.Currency)
                    .HasConversion<CurrencyCodeConverter>()
                    .HasColumnName("TotalTaxCurrency").HasMaxLength(3).IsRequired();
            });

            builder.OwnsMany(document => document.Lines, line =>
            {
                line.ToTable("TaxDocumentLines", OrderingDbContext.DeliverySchema);
                line.WithOwner().HasForeignKey("DocumentNumber");
                line.HasKey("DocumentNumber", "Seq");

                line.Property(value => value.Seq).ValueGeneratedNever();
                line.Property(value => value.LineType).HasMaxLength(32).IsRequired();
                line.Property(value => value.Code).HasMaxLength(64).IsRequired();
                line.Property(value => value.Description).HasMaxLength(256).IsRequired();
                line.Property(value => value.TaxRate).HasColumnType("decimal(9,6)");
                line.Property(value => value.TaxJurisdiction).HasMaxLength(32);

                line.OwnsOne(value => value.NetAmount, money =>
                {
                    money.Property(amount => amount.Amount)
                        .HasColumnName("NetAmount").HasColumnType("decimal(19,4)").IsRequired();
                    money.Property(amount => amount.Currency)
                        .HasConversion<CurrencyCodeConverter>()
                        .HasColumnName("Currency").HasMaxLength(3).IsRequired();
                });

                line.OwnsOne(value => value.TaxAmount, money =>
                {
                    money.Property(amount => amount.Amount)
                        .HasColumnName("TaxAmount").HasColumnType("decimal(19,4)").IsRequired();
                    money.Property(amount => amount.Currency)
                        .HasConversion<CurrencyCodeConverter>()
                        .HasColumnName("TaxCurrency").HasMaxLength(3).IsRequired();
                });
            });

            builder.OwnsMany(document => document.SubmissionAttempts, attempt =>
            {
                attempt.ToTable("TaxDocumentSubmissionAttempts", OrderingDbContext.DeliverySchema);
                attempt.WithOwner().HasForeignKey("DocumentNumber");
                attempt.HasKey("DocumentNumber", "Seq");

                attempt.Property(value => value.Seq).ValueGeneratedNever();
                attempt.Property(value => value.AttemptedAtUtc)
                    .HasConversion<InstantConverter>().HasColumnType("datetime2(7)").IsRequired();
                attempt.Property(value => value.Outcome).HasMaxLength(32).IsRequired();
                attempt.Property(value => value.ResponseReference).HasMaxLength(128);
                attempt.Property(value => value.RejectionReason).HasMaxLength(512);
            });
        }
    }
}
