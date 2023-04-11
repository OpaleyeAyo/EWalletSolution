using System;

namespace EWallet.DataLayer.DTO.Response
{
    public class GetSettlementAccountResponseDto
    {
        public Guid Id { get; set; } 

        public string AccountName { get; set; }

        public string AccountNumber { get; set; }

        public string CurrencyCode { get; set; }

        public decimal AccountBalance { get; set; }

        public DateTime DateCreated { get; set; }
    }
}
