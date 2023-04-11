using System;

namespace EWallet.Entities.DbEntities
{
    public class CurrencyLogo
    {
        public Guid Id { get; set; } = new Guid();

        public string Url { get; set; }

        public string PublicId { get; set; }

        public DateTime DateUploaded { get; set; } = DateTime.UtcNow;

        public DateTime DateUpdated { get; set; }

        //Navigation property
        public Guid CurrencyId { get; set; }
        public Currency Currency { get; set; }
    }
}
