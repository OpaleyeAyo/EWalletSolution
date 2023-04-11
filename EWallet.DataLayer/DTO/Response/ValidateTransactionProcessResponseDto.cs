

using EWallet.Entities.DbEntities;

namespace EWallet.DataLayer.DTO.Response
{
    public class ValidateTransactionProcessResponseDto
    {
        public Wallet Wallet { get; set; }

        public SettlementAccount SettlementAccount { get; set; }

        public Customer Customer { get; set; }

        public Currency Currency { get; set; }
    }
}
