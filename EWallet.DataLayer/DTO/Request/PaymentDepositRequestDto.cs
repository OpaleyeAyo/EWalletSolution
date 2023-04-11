using System.ComponentModel.DataAnnotations;

namespace EWallet.DataLayer.DTO.Request
{
    public class PaymentDepositRequestDto
    {
        [Required]
        public string AccountNumber { get; set; }
        
        [Required]
        public decimal Amount { get; set; }
    }
}
