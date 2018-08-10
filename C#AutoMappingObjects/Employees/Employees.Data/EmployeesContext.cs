using Employees.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace Employees.Data
{
    public class EmployeesContext:DbContext
    {
        public EmployeesContext()
        {

        }

        public EmployeesContext(DbContextOptions options)
          : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Configuration.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Employee>()
                .HasKey(e => e.Id);

            builder.Entity<Employee>()
                .Property(e => e.FirstName)
                .IsRequired(true)
                .IsUnicode(true)
                .HasMaxLength(30);

            builder.Entity<Employee>()
              .Property(e => e.LastName)
              .IsRequired(true)
              .IsUnicode(true)
              .HasMaxLength(30);

            builder.Entity<Employee>().HasOne(e => e.Manager)
               .WithMany(e => e.ManagedEmployees)
.HasForeignKey(e => e.ManagerId);


        }
    }
}
