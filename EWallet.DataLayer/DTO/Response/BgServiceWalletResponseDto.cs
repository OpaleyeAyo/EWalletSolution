using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EWallet.DataLayer.DTO.Response
{
    public class BgServiceWalletResponseDto
    {
        public decimal AccountBalance { get; set; }

        public DateTime DateOfInterestAddition { get; set; }
    }
}
