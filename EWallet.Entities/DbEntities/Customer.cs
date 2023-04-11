using System;
using System.Collections.Generic;

namespace EWallet.Entities.DbEntities
{
    public class Customer
    {
        public Guid Id { get; set; } = new Guid();

        public Guid IdentityId { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public DateTime DateUpdated { get; set; }

        public bool IsActive { get; set; } = true;

        public string FirstName { get; set; }
        
        public string LastName { get; set; }
        
        public string Email { get; set; }

        public ProfilePhoto ProfilePhoto { get; set; }

        public ICollection<Wallet> Wallets { get; set; }
    }
}
