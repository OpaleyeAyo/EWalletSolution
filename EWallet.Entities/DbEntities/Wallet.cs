using EWallet.Utility.HelperMethods;
using System;
using System.Collections.Generic;

namespace EWallet.Entities.DbEntities
{
    public class Wallet
    {
        public Guid Id { get; set; } = new Guid();

        public string AccountNumber { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public DateTime DateUpdated { get; set; }

        public decimal AccountBalance { get; set; } = 0;

        public DateTime DateOfInterestAddition { get; set; }

        //Navigation property
        public Guid CustomerId { get; set; } 
        
        public Customer Customer { get; set; }

        public Currency Currency { get; set; }

        public ICollection<Transaction> Transactions { get; set; }

        public Wallet()
        {
            AccountNumber = GenerateAccountNumber.GenerateNumber();
            DateOfInterestAddition = DateCreated.AddYears(1);
        }
    }
}
