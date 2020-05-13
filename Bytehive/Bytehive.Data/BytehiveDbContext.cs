using Bytehive.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;

namespace Bytehive.Data
{
    public class BytehiveDbContext : DbContext
    {
        public BytehiveDbContext(DbContextOptions<BytehiveDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<ScrapeRequest> ScrapeRequests { get; set; }

        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public DbSet<UserRole> UserRoles { get; set; }

        public DbSet<FAQ> FAQs { get; set; }

        public DbSet<FAQCategory> FAQCategories { get; set; }

        public DbSet<Payment> Payments { get; set; }

        public DbSet<PaymentTier> PaymentTiers { get; set; }

        public DbSet<File> Files { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>().ToTable("user");
            builder.Entity<UserRole>().ToTable("user_role");
            builder.Entity<Role>().ToTable("role");
            builder.Entity<FAQ>().ToTable("faq");
            builder.Entity<FAQCategory>().ToTable("faq_category");
            builder.Entity<Payment>().ToTable("payment");
            builder.Entity<PaymentTier>().ToTable("payment_tier");
            builder.Entity<ScrapeRequest>().ToTable("scrape_request");
            builder.Entity<File>().ToTable("file");
            builder.Entity<RefreshToken>().ToTable("refresh_token");

            builder
                .Entity<User>()
                .HasIndex(u => new { u.Email, u.Provider })
                .IsUnique();

            builder
                .Entity<ScrapeRequest>()
                .HasOne(sr => sr.User)
                .WithMany(u => u.ScrapeRequests)
                .HasForeignKey(sr => sr.UserId);

            builder
                .Entity<RefreshToken>()
                .HasOne(sr => sr.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(sr => sr.UserId);

            builder
                .Entity<Payment>()
                .HasOne(sr => sr.User)
                .WithMany(u => u.Payments)
                .HasForeignKey(sr => sr.UserId);

            builder
                .Entity<Payment>()
                .HasOne(f => f.PaymentTier)
                .WithMany(u => u.Payments)
                .HasForeignKey(f => f.PaymentTierId);

            builder
                .Entity<FAQ>()
                .HasOne(f => f.Category)
                .WithMany(u => u.FAQs)
                .HasForeignKey(f => f.CategoryId);

            builder
                .Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            builder
                .Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);

            builder
                .Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            builder.Entity<ScrapeRequest>()
                .HasOne(sr => sr.File)
                .WithOne(f => f.ScrapeRequest)
                .HasForeignKey<File>(b => b.ScrapeRequestId);

            base.OnModelCreating(builder);
        }
    }
}
