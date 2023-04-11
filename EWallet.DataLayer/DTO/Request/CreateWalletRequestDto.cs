using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EWallet.DataLayer.DTO.Request
{
    public class CreateWalletRequestDto
    {
        [Required]
        public string CurrencyCode { get; set; }
    }
}
