using AeroTech.Framework.Core.Domain.Repository;
using AeroTech.Framework.Core.ServiceContracts;
using AeroTech.Framework.Infrastructure.Persistence;
using AeroTech.Ordering.Domain.DeliveryRecordAggregate;
using AeroTech.Ordering.Domain.DocumentNumberRangeAggregate;
using AeroTech.Ordering.Domain.GroupBookingAggregate;
using AeroTech.Ordering.Domain.OperationAggregate;
using AeroTech.Ordering.Domain.OrderAggregate;
using AeroTech.Ordering.Domain.TaxDocumentAggregate;
using AeroTech.Ordering.Persistence.Inbox;
using AeroTech.Ordering.Persistence.Outbox;
using Microsoft.EntityFrameworkCore;

namespace AeroTech.Ordering.Persistence
{
    public sealed class OrderingDbContext : CommandDbContext, IUnitOfWork
    {
        public const string MigrationsHistorySchema = "dbo";
        public const string MigrationsHistoryTable = "__CommandsMigrationHistory";

        public const string OrderingSchema = "Ordering";
        public const string DeliverySchema = "Delivery";
        public const string OperationsSchema = "Ops";
        public const string AuditSchema = "Audit";

        public OrderingDbContext(
            DbContextOptions<OrderingDbContext> options,
            IIdentityService identityService,
            IClock clock,
            IDomainEventDispatcher domainEventDispatcher)
            : base(options, identityService, clock, domainEventDispatcher)
        {
        }

        public DbSet<Order> Orders => Set<Order>();

        public DbSet<GroupBooking> GroupBookings => Set<GroupBooking>();

        public DbSet<DeliveryRecord> DeliveryRecords => Set<DeliveryRecord>();

        public DbSet<TaxDocument> TaxDocuments => Set<TaxDocument>();

        public DbSet<DocumentNumberRange> DocumentNumberRanges => Set<DocumentNumberRange>();

        public DbSet<Operation> Operations => Set<Operation>();

        public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

        public DbSet<InboxMessage> InboxMessages => Set<InboxMessage>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(OrderingSchema);
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderingDbContext).Assembly);
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<decimal>().HavePrecision(19, 4);
            configurationBuilder.Properties<string>().HaveMaxLength(256);
        }
    }
}
