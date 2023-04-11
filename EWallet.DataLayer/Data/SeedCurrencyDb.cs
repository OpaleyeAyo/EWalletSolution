using EWallet.DataLayer.DTO.Generic;
using EWallet.Entities.DbEntities;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EWallet.DataLayer.Data
{
    public static class SeedCurrencyDb
    {
        public static void SeedCurrency(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>());
            }
        }

        private static void SeedData(AppDbContext context)
        {
            try
            {
                if (!context.Currencies.Any())
                {
                    Console.WriteLine("Seeding Currency database...");

                    context.Currencies.AddRange(
                        new Currency()
                        {
                            NameOfCurrency = NameOfCurrency.BritishPound,
                            CurrencyCode = CurrencyCode.BritishPound
                        },
                        new Currency()
                        {
                            NameOfCurrency = NameOfCurrency.NigerianNaira,
                            CurrencyCode = CurrencyCode.NigerianNaira
                        },
                        new Currency()
                        {
                            NameOfCurrency = NameOfCurrency.USDollar,
                            CurrencyCode = CurrencyCode.USDollar
                        }
                    );

                    context.SaveChanges();

                    Console.WriteLine("Seeding database completed...");
                }
                else
                {
                    Console.WriteLine("Currencies already available...");

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Something went wrong : {ex.Message}");
            }
        }
    }
}
