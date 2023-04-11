using EWallet.Entities.DbEntities;
using System;
using System.Collections.Generic;

namespace EWallet.Test.MockData
{
    public class WalletMockData
    {
        public static List<Wallet> GetWallets()
        {
            return new List<Wallet>
            {
                new Wallet
                {
                    Id = new Guid(),
                    AccountNumber = "517392232517",
                    DateCreated = DateTime.UtcNow,
                    DateUpdated = DateTime.UtcNow,
                    AccountBalance = 322983,
                    DateOfInterestAddition = DateTime.UtcNow,
                },
                new Wallet
                {
                    Id = new Guid(),
                    AccountNumber = "217392232517",
                    DateCreated = DateTime.UtcNow,
                    DateUpdated = DateTime.UtcNow,
                    AccountBalance = 52203,
                    DateOfInterestAddition = DateTime.UtcNow,
                },
                new Wallet
                {
                    Id = new Guid(),
                    AccountNumber = "611392232517",
                    DateCreated = DateTime.UtcNow,
                    DateUpdated = DateTime.UtcNow,
                    AccountBalance = 325623,
                    DateOfInterestAddition = DateTime.UtcNow,
                },

            };
        }

        public static Wallet GetWallet()
        {
            return new Wallet
            {
                Id = new Guid(),
                AccountNumber = "517392232517",
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow,
                AccountBalance = 322983,
                DateOfInterestAddition = DateTime.UtcNow,
            };
        }
    }
}
