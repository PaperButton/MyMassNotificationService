using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MyMassNotificationService.Domain.Entities;

namespace MyMassNotificationService.Infrastructure.Data.Configurations
{
    public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
    {
        public void Configure(EntityTypeBuilder<OutboxMessage> builder)
        {
            builder.HasKey(m => m.Id);
            builder.Property(m => m.Topic).IsRequired();
            builder.Property(m => m.Key).IsRequired();
            builder.Property(m => m.Value).IsRequired();
            builder.Property(m => m.CreatedAt).IsRequired();
            builder.Property(m => m.IsProcessed).IsRequired();
        }
    }
}
