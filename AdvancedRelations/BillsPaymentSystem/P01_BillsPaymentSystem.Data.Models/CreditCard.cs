using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace P01_BillsPaymentSystem.Data.Models
{
    public class CreditCard
    {
        public CreditCard() { }

        public CreditCard(DateTime expirattionDate, decimal limit, decimal moneyOwed)
        {
            this.ExpirationDate = expirattionDate;
            this.Limit = limit;
            this.MoneyOwed = moneyOwed;
        }

        public int CreditCardId { get; set; }

        public decimal Limit { get; set; }

        public decimal MoneyOwed { get; set; }

       
        public decimal LimitLeft => this.Limit - this.MoneyOwed;

        public DateTime ExpirationDate { get; set; }

        public int PaymentMethodId { get; set; }

        public PaymentMethod PaymentMethod { get; set; }


        public void Withdraw(decimal money)
        {
            if (money > Limit)
            {
                throw new ArgumentException("Insufficient funds!");
            }
            this.MoneyOwed += money;
        }

        public void Deposit(decimal money)
        {
            if (money < 0)
            {
                throw new ArgumentException("Value cannot be negative!");
            }
            this.MoneyOwed -= money;
        }
    }
}