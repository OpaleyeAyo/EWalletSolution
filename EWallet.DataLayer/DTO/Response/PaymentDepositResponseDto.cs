using EWallet.Entities.Enums;
using System;

namespace EWallet.DataLayer.DTO.Response
{
    public class PaymentDepositResponseDto
    {
        public Guid Id { get; set; }

        public string TransactionReference { get; set; }

        public string TransactionSourceAccount { get; set; }

        public string TransactionDestinationAccount { get; set; }

        public decimal Amount { get; set; }

        public DateTime TransactionDate { get; set; }

        public TransactionType TypeOfTransaction { get; set; }

        public TransactionStatus TransactionStatus { get; set; }
    }
}
