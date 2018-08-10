using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using P01_BillsPaymentSystem.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace P01_BillsPaymentSystem.Data.Configurations
{
    public class BankAccountConfiguration : IEntityTypeConfiguration<BankAccount>
    {
        public void Configure(EntityTypeBuilder<BankAccount> builder)
        {
            builder.HasKey(b => b.BankAccountId);

            builder
                .Property(b => b.Balance)
                .IsRequired();

            builder
                .Property(x => x.BankName)
                .HasColumnType("nvarchar(50)")
                .IsRequired();

            builder
                .Property(x => x.SwiftCode)
                .HasColumnType("varchar(20)")
                .IsRequired();

            builder
                .Ignore(b => b.PaymentMethodId);
        }

       
    }
}
