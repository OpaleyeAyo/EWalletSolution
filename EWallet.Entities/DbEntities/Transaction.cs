using EWallet.Entities.Enums;
using System;

namespace EWallet.Entities.DbEntities
{
    public class Transaction
    {
        public Guid Id { get; set; } = new Guid();

        public string TransactionReference { get; set; }

        public string TransactionSourceAccount { get; set; }

        public string TransactionDestinationAccount { get; set; }

        public string CurrencyCode { get; set; }

        public decimal Amount { get; set; }

        public Guid CustomerIdentityId { get; set; }

        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

        public TransactionType TypeOfTransaction { get; set; }

        public TransactionStatus TransactionStatus { get; set; }

        public Guid WalletId { get; set; }

        public Wallet Wallet { get; set; }

        public Transaction()
        {
            TransactionReference = $"{(Guid.NewGuid().ToString().Replace("-", "").Substring(1, 27)).ToUpper()}";
        }
    }
}
