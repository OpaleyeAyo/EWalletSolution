using EWallet.Utility.HelperMethods;
using System;

namespace EWallet.Entities.DbEntities
{
    public class SettlementAccount
    {
        public Guid Id { get; set; } = new Guid();

        public string AccountName { get; set; }

        public string AccountNumber { get; set; }

        public decimal AccountBalance { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public DateTime DateUpdated { get; set; }

        //Navigation property
        public Guid CurrencyId { get; set; }
        public Currency Currency { get; set; }

        public SettlementAccount()
        {
            AccountNumber = GenerateAccountNumber.GenerateNumber();
        }
    }
}
