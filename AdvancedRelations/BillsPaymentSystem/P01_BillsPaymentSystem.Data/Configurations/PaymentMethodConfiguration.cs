using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using P01_BillsPaymentSystem.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace P01_BillsPaymentSystem.Data.Configurations
{
    public class PaymentMethodConfiguration : IEntityTypeConfiguration<PaymentMethod>
    {
        public void Configure(EntityTypeBuilder<PaymentMethod> builder)
        {
            builder.HasKey(p => p.Id);

            builder
                .Property(x => x.Type).IsRequired();

            builder
                .Property(x => x.UserId).IsRequired();

            builder
                .HasOne(x => x.BankAccount)
                .WithOne(x => x.PaymentMethod)
                .HasForeignKey<PaymentMethod>(x => x.BankAccountId);

            builder
                .HasOne(x => x.CreditCard)
                .WithOne(x => x.PaymentMethod)
                .HasForeignKey<PaymentMethod>(x => x.CreditCardId);

            builder
                .HasIndex(x => new
                {
                    x.UserId,
                    x.BankAccountId,
                    x.CreditCardId
                })
            .IsUnique();
        }

        
    }
}
