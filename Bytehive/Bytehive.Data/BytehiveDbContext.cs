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

        public DbSet<ScrapeRequest> ScrapeRequests { get; set; }

        public DbSet<FAQ> FAQs { get; set; }

        public DbSet<FAQCategory> FAQCategories { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>().ToTable("user");
            builder.Entity<FAQ>().ToTable("faq");
            builder.Entity<FAQCategory>().ToTable("faq_category");
            builder.Entity<ScrapeRequest>().ToTable("scrape_request");
            builder.Entity<RefreshToken>().ToTable("refresh_token");

            builder
                .Entity<User>()
                .HasIndex(u => u.Email)
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
                .Entity<FAQ>()
                .HasOne(f => f.Category)
                .WithMany(u => u.FAQs)
                .HasForeignKey(f => f.CategoryId);

            base.OnModelCreating(builder);
        }
    }
}
