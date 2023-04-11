using System;

namespace EWallet.Entities.DbEntities
{
    public class ProfilePhoto
    {
        public Guid Id { get; set; } = new Guid();

        public string Url { get; set; }
        
        public string PublicId { get; set; }

        public DateTime DateUploaded { get; set; } = DateTime.UtcNow;

        public DateTime DateUpdated { get; set; }

        //navigation property
        public Guid CustomerId { get; set; }

        public Customer Customer { get; set; }
    }
}
