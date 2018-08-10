﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using P01_BillsPaymentSystem.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace P01_BillsPaymentSystem.Data.Configurations
{
    public class CreditCardConfiguration : IEntityTypeConfiguration<CreditCard>
    {
        public void Configure(EntityTypeBuilder<CreditCard> builder)
        {
            builder.HasKey(c => c.CreditCardId);

            builder
                .Property(c => c.Limit)
                .IsRequired();

            builder
                .Property(c => c.MoneyOwed)
                .IsRequired();

            builder
                .Property(c => c.ExpirationDate)
                .IsRequired();

            builder
                .Ignore(c => c.LimitLeft);

            builder
                .Ignore(c => c.PaymentMethodId);
        }

    }
}
