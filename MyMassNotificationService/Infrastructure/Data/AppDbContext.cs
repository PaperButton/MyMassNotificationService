using MyMassNotificationService.Domain.Entities;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MyMassNotificationService.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { 

            this.Database.EnsureCreated();

        }

        public DbSet<OutboxMessage> OutboxMessages { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<NotificationTemplate> NotificationTemplates { get; set; }
        public DbSet<NotificationRecord> NotificationRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NotificationRecord>()
                .HasOne(n => n.User)
                .WithMany()
                .HasForeignKey(n => n.UserId);

            modelBuilder.Entity<NotificationRecord>()
                .HasOne(n => n.NotificationTemplate)
                .WithMany()
                .HasForeignKey(n => n.NotificationTemplateId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
