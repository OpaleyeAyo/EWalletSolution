using EWallet.DataLayer.DTO.Generic;
using EWallet.Entities.DbEntities;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace EWallet.DataLayer.Data
{
    public static class SeedSettlementAccountDb
    {
        public static void SeedSettlementDb(IApplicationBuilder app)
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
                if (!context.SettlementAccounts.Any())
                {
                    var USDCurrency = context.Currencies
                         .Where(x => x.CurrencyCode == CurrencyCode.USDollar)
                         .FirstOrDefault();

                    var NGNCurrency = context.Currencies
                         .Where(x => x.CurrencyCode == CurrencyCode.NigerianNaira)
                         .FirstOrDefault();

                    var GBPCurrency = context.Currencies
                         .Where(x => x.CurrencyCode == CurrencyCode.BritishPound)
                         .FirstOrDefault();

                    Console.WriteLine("Seeding Settlement accounts database...");

                    context.SettlementAccounts.AddRange(
                        new SettlementAccount()
                        {
                            AccountName = SettlementAccountName.USDollarAccount,
                            AccountBalance = 30000000000000,
                            CurrencyId = USDCurrency.Id,
                            Currency = USDCurrency
                        },
                        new SettlementAccount()
                        {
                            AccountName = SettlementAccountName.NigerianNairaAccount,
                            AccountBalance = 30000000000000,
                            CurrencyId = NGNCurrency.Id,
                            Currency = NGNCurrency
                        },
                        new SettlementAccount()
                        {
                            AccountName = SettlementAccountName.BritishPoundAccount,
                            AccountBalance = 30000000000000,
                            CurrencyId = GBPCurrency.Id,
                            Currency = GBPCurrency
                        }
                    ) ;

                    context.SaveChanges();

                    Console.WriteLine("Seeding database completed...");
                }
                else
                {
                    Console.WriteLine("Settlement accounts already available...");

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Something went wrong : {ex.Message}");
            }
        }
    }

    
}
