using EWallet.Entities.DbEntities;
using System;
using System.Collections.Generic;

namespace EWallet.Test.MockData
{
    public class TransactionMockData
    {
        public static List<Transaction> GetTransactions()
        {
            return new List<Transaction>
            {
                new Transaction
                {
                    Id = new Guid(),
                    TransactionReference = "3027fd86-6088-453b-b44d-517392232517",
                    TransactionSourceAccount = "517392232517",
                    TransactionDestinationAccount = "217392232518",
                    CurrencyCode = "DTO",
                    Amount = 3223,
                },
                new Transaction
                {
                    Id = new Guid(),
                    TransactionReference = "2027fd86-6088-453b-b44d-617392232517",
                    TransactionSourceAccount = "717392232517",
                    TransactionDestinationAccount = "817392232518",
                    CurrencyCode = "SCT",
                    Amount = 32223,
                },
                new Transaction
                {
                    Id = new Guid(),
                    TransactionReference = "3027fd82-6088-953b-b44d-517392232517",
                    TransactionSourceAccount = "517392832517",
                    TransactionDestinationAccount = "217302232518",
                    CurrencyCode = "DAZ",
                    Amount = 4670,
                },
            };
        }

        public static Transaction AddTransaction()
        {
            return new Transaction
            {
                Id = new Guid(),
                TransactionReference = "3027fd86-6088-453b-b44d-517392232517",
                TransactionSourceAccount = "517392232517",
                TransactionDestinationAccount = "217392232518",
                CurrencyCode = "DTO",
                Amount = 3223,
            };
        }

        public static Transaction GetTransaction()
        {
            return new Transaction
            {
                Id = new Guid(),
                TransactionReference = "3027fd86-6088-453b-b44d-517392232517",
                TransactionSourceAccount = "517392232517",
                TransactionDestinationAccount = "217392232518",
                CurrencyCode = "DTO",
                Amount = 3223,
            };
            
        }
    }
}
