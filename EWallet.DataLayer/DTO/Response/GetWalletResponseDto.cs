using System;
using System.Collections.Generic;

namespace EWallet.DataLayer.DTO.Response
{
    public class GetWalletResponseDto
    {
        public Guid Id { get; set; } 

        public string AccountNumber { get; set; }

        public string CurrencyCode { get; set; }

        public DateTime DateCreated { get; set; } 

        public decimal AccountBalance { get; set; }

        public IEnumerable<GetTransactionResponseDto> Transactions { get; set; }
    }
}
