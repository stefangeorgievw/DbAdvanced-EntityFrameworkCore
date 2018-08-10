using System;

namespace P01_BillsPaymentSystem.Data.Models
{
    public class BankAccount
    {
        public BankAccount() { }

        public BankAccount(decimal balance, string bankName, string swiftCode)
        {
            this.Balance = balance;
            this.BankName = bankName;
            this.SwiftCode = swiftCode;
        }

        public int BankAccountId { get; set; }

        public decimal Balance { get; set; }

        public string BankName { get; set; }

        public string  SwiftCode { get; set; }

        public int PaymentMethodId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }


        public void Withdraw(decimal money)
        {
            if (money > Balance)
            {
                throw new ArgumentException("Insufficient funds!");
            }
            this.Balance -= money;
        }

        public void Deposit(decimal money)
        {
            if (money < 0)
            {
                throw new ArgumentException("Value cannot be negative !");
            }
            this.Balance += money;
        }
    }
}