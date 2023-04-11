using EWallet.Entities.DbEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EWallet.Test.MockData
{
    public class CurrencyMockData
    {
        public static List<Currency> GetCurrencies()
        {
            return new List<Currency>
            {
                new Currency
                {
                    Id = new Guid(),
                    DateCreated = DateTime.UtcNow,
                    DateUpdated = DateTime.UtcNow,
                    NameOfCurrency = "Nigerian Naira",
                    CurrencyCode = "NGN"
                },
                new Currency
                {
                    Id = new Guid(),
                    DateCreated = DateTime.UtcNow,
                    DateUpdated = DateTime.UtcNow,
                    NameOfCurrency = "American Dollar",
                    CurrencyCode = "USD"
                },
                new Currency
                {
                    Id = new Guid(),
                    DateCreated = DateTime.UtcNow,
                    DateUpdated = DateTime.UtcNow,
                    NameOfCurrency = "Australian Dollar",
                    CurrencyCode = "AUD"
                }

            };
        }

        public static Currency GetCurrency()
        {
            return new Currency
            {
                Id = new Guid(),
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow,
                NameOfCurrency = "Nigerian Naira",
                CurrencyCode = "NGN"
            };
        }
    }
}
