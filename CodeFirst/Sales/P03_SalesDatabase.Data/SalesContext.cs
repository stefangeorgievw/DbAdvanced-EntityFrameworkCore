using Microsoft.EntityFrameworkCore;
using P03_SalesDatabase.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace P03_SalesDatabase.Data
{
    class SalesContext : DbContext
    {
        public SalesContext() { }

        public SalesContext(DbContextOptions options)
            : base(options) { }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<Sale> Sales { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Cofiguration.connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.CustomerId);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .IsUnicode()
                    .HasMaxLength(100);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .IsUnicode(false)
                    .HasMaxLength(80);
            });

            modelBuilder.Entity<Product>(entity =>
            {


                entity.Property(e => e.Name)
                    .IsRequired()
                    .IsUnicode()
                    .HasMaxLength(50);

               entity.Property(e => e.Description)
                   .HasMaxLength(250)
                   .HasDefaultValue("No description");
            });

            modelBuilder.Entity<Store>(entity =>
            {


                entity.Property(e => e.Name)
                    .IsRequired()
                    .IsUnicode()
                    .HasMaxLength(80);
            });

            modelBuilder.Entity<Sale>(entity =>
            {
                entity.HasKey(e => e.SaleId);

                entity.Property(e => e.Date)
                 .HasDefaultValueSql("GETDATE()");

                entity.HasOne(e => e.Product)
                    .WithMany(p => p.Sales)
                    .HasForeignKey(e => e.ProductId);



                entity.HasOne(e => e.Customer)
                    .WithMany(c => c.Sales)
                    .HasForeignKey(e => e.CustomerId);


                entity.HasOne(e => e.Store)
                    .WithMany(s => s.Sales)
                    .HasForeignKey(e => e.StoreId);

            });
        }
    }
}
