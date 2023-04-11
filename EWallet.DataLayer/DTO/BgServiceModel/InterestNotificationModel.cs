using System;
using System.Collections.Generic;

namespace EWallet.DataLayer.DTO.BgServiceModel
{
    public class InterestNotificationModel
    {
        public string CustomerEmail { get; set; }

        public string Name { get; set; }

        public string CurrencyCode { get; set; }

        public string InterestAmount { get; set; }

        public string NewAccountBalance { get; set; }

        public string PreviousAccountBalance { get; set; }

        public DateTime DateOfTransaction { get; set; }

        public List<string> ListOfAdminEmail { get; set; }
    }
}
