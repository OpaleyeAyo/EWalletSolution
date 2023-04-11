using System;

namespace EWallet.Entities.DbEntities
{
    public class Currency
    {
        public Guid Id { get; set; } = new Guid();

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public DateTime DateUpdated { get; set; }

        public string NameOfCurrency { get; set; }

        public string CurrencyCode { get; set; }

        public CurrencyLogo CurrencyLogo { get; set; }
    }
}
