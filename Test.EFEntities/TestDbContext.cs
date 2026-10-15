using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Test.EFEntities
{
    public class TestDbContext : DbContext
    {

        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SourceUser>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                // Configure other properties and relationships as needed
            });

            base.OnModelCreating(modelBuilder);
            // Configure your entity mappings here
        }

        public DbSet<SourceUser> SourceUsers => Set<SourceUser>();
    }
}
