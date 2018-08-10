using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using P01_HospitalDatabase.Data.Models;
using System;

namespace P01_HospitalDatabase.Data
{
    public class HospitalContext : DbContext
    {
        public HospitalContext(DbContextOptions options) : base(options)
        {
        }

        public HospitalContext()
        {
        }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<Diagnose> Diagnoses { get; set; }
        public DbSet<Medicament> Medicaments { get; set; }
        public DbSet<Visitation> Visitations { get; set; }
        public DbSet<PatientMedicament> PatientsMedicaments { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Configure.connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Patient>(e =>
            {
                e.Property(p => p.FirstName)
                .HasMaxLength(50)
                .IsUnicode();

                e.Property(p => p.LastName)
               .HasMaxLength(50)
               .IsUnicode();

                e.Property(p => p.Address)
               .HasMaxLength(250)
               .IsUnicode();

                e.Property(p => p.Email)
               .HasMaxLength(80)
               .IsUnicode(false);

                e.HasMany(p => p.Visitations)
                .WithOne(p => p.Patient)
                .HasForeignKey(p => p.PatientId);

                e.HasMany(p => p.Diagnoses)
               .WithOne(p => p.Patient)
               .HasForeignKey(p => p.PatientId);

                e.HasMany(p => p.Prescriptions)
               .WithOne(p => p.Patient)
               .HasForeignKey(p => p.PatientId);




            });

            builder.Entity<Diagnose>(entity =>
            {
                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(true);

                entity.Property(e => e.Comments)
                    .HasMaxLength(250)
                    .IsUnicode(true);

            });

            builder.Entity<Medicament>(entity =>
            {

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(true);

                entity.HasMany(p => p.Prescriptions)
                  .WithOne(p => p.Medicament)
                  .HasForeignKey(p => p.MedicamentId);
            });

            builder.Entity<PatientMedicament>(entity =>
            {
                entity.HasKey(e => new { e.PatientId, e.MedicamentId });

              
            });

            builder.Entity<Doctor>(entity =>
            {
                

                entity.Property(e => e.Name)
                    .IsRequired()
                    .IsUnicode()
                    .HasMaxLength(100);

                entity.Property(e => e.Specialty)
                    .IsRequired()
                    .IsUnicode()
                    .HasMaxLength(100);
            });

        }
    }
}
