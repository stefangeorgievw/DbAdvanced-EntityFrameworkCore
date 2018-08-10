using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using ProductShop.Data.Config;
using ProductShop.Models;
using System;

namespace ProductShop.Data
{
    public class ProductShopContext : DbContext
    {
        public ProductShopContext()
        {

        }

        public ProductShopContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CategoryProducts> CategoryProducts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Configuration.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<CategoryProducts>()
                .HasKey(e => new { e.ProductId, e.CategoryId });

            builder.Entity<CategoryProducts>()
                   .HasOne(cp => cp.Product)
                   .WithMany(p => p.CategoryProducts)
                   .HasForeignKey(cp => cp.ProductId);

            builder.Entity<CategoryProducts>()
                .HasOne(cp => cp.Category)
                .WithMany(c => c.CategoryProducts)
.HasForeignKey(cp => cp.CategoryId);


            builder.Entity<User>()
                .HasMany(u => u.BoughtProducts)
                .WithOne(p => p.Buyer)
                .HasForeignKey(p => p.BuyerId);


            builder.Entity<User>()
                .HasMany(u => u.SelledProducts)
                .WithOne(p => p.Seller)
                .HasForeignKey(p => p.SellerId);
        }
    }
}
