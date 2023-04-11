using EWallet.API.AsyncDataTransfer;
using EWallet.DataLayer.Contracts;
using EWallet.DataLayer.Data;
using EWallet.DataLayer.DTO.BgServiceModel;
using EWallet.DataLayer.DTO.Generic;
using EWallet.Entities.DbEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EWallet.API.BgImplementation
{
    public class BgServices : IBackgroundServiceRepository
    {
        private readonly ILogger<BgServices> _logger;
        private readonly IMessageBusClient _messageBusClient;
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public BgServices(ILogger<BgServices> logger,
            UserManager<IdentityUser> userManager,
            IMessageBusClient messageBusClient, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;
            _messageBusClient = messageBusClient;
        }

        public async Task AddInterest()
        {
            try
            {
                var wallets = await _context.Wallets
                    //.Where(x => x.AccountBalance > 0 )
                    .Where(x => x.AccountBalance > 0 && x.DateOfInterestAddition == DateTime.UtcNow)
                    .ToListAsync();

                _logger.LogInformation($"There are {wallets.Count} wallet for update today {DateTime.UtcNow}");

                foreach (var wallet in wallets)
                {
                    var customer = await _context.Customers
                        .Where(x => x.Id == wallet.CustomerId)
                        .FirstOrDefaultAsync();

                    var currency = await _context.Wallets
                        .Where(x => x.Id == wallet.Id)
                        .Select(x => x.Currency)
                        .FirstOrDefaultAsync();

                    //get settlement account for the currency type

                    var settlementAccount = await _context.SettlementAccounts
                        .Where(x => x.CurrencyId == currency.Id)
                        .FirstOrDefaultAsync();

                    _logger.LogInformation($"Settlement Account: {JsonSerializer.Serialize(settlementAccount)}");



                    _logger.LogInformation($"Wallet balance before adding interest: {wallet.AccountBalance}");

                    double principal = Convert.ToDouble(wallet.AccountBalance.ToString());

                    _logger.LogInformation($"Principal : {principal}.");
                    _logger.LogInformation($"Rate : {3.75} percent.");
                    _logger.LogInformation($"Time : {1} year.");

                    _logger.LogInformation("Calculating intereset...");
                    decimal simpleInterest = (decimal)(CalcInterest(principal, 3.75, 1));

                    _logger.LogInformation($"Interest: {simpleInterest}");

                    wallet.AccountBalance += simpleInterest;
                    wallet.DateOfInterestAddition = wallet.DateOfInterestAddition.AddYears(1);
                    wallet.DateUpdated = DateTime.UtcNow;

                    var updateUserDb = await _context.SaveChangesAsync();

                    settlementAccount.AccountBalance -= simpleInterest;
                    settlementAccount.DateCreated = DateTime.UtcNow;

                    var updateSettlementAccount = await _context.SaveChangesAsync();

                    if (updateUserDb >= 0 && updateSettlementAccount >= 0)
                    {
                        _logger.LogInformation($"Wallet with id of {wallet.Id} and account number of {wallet.AccountNumber} has been updated with {simpleInterest}.");
                        _logger.LogInformation($"Settlementment account with id of {settlementAccount.Id} and account number of {settlementAccount.AccountNumber} has been updated with {simpleInterest}.");
                    }

                    var transaction = new Transaction() 
                    {
                        TransactionSourceAccount = settlementAccount.AccountNumber,
                        TransactionDestinationAccount = wallet.AccountNumber,
                        CurrencyCode = currency.CurrencyCode,
                        Amount = simpleInterest,
                        TypeOfTransaction = Entities.Enums.TransactionType.INTERESTCREDIT,
                        TransactionStatus = Entities.Enums.TransactionStatus.Success,
                        WalletId = wallet.Id,
                        CustomerIdentityId = customer.IdentityId
                    };

                    var createTransaction = await _context.Transactions.AddAsync(transaction);

                    var isTransactionSAved = await _context.SaveChangesAsync();

                    if (isTransactionSAved >= 0 )
                    {
                        _logger.LogInformation($"Transaction with the reference number saved {createTransaction.Entity.TransactionReference}.");
                    }

                    string interestString = simpleInterest.ToString();
                    string prevBalance = wallet.AccountBalance.ToString();
                    var newBalance = (wallet.AccountBalance + simpleInterest).ToString();

                    _logger.LogInformation($"New Acount BAL: ========> {newBalance}");

                    var adminList = await GetAdminMailingList();

                    InterestNotificationModel notificationModel = new InterestNotificationModel
                    {
                        CustomerEmail = customer.Email,
                        Name = $"{customer.FirstName} {customer.LastName}",
                        InterestAmount = interestString,
                        PreviousAccountBalance = prevBalance,
                        NewAccountBalance = newBalance,
                        CurrencyCode = currency.CurrencyCode,
                        DateOfTransaction = DateTime.UtcNow,
                        ListOfAdminEmail = adminList
                    };

                    try
                    {
                        var eventModel = new GenericEventDto<InterestNotificationModel>()
                        {
                            EventType = AsyncEventType.SendInterestAdditionNotification,
                            EventModel = notificationModel
                        };

                        _messageBusClient.PubMessage(eventModel);

                        _logger.LogInformation("New Notification message published.");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Could not publish message.");
                        _logger.LogError($"Something went wrong with the message bus: {ex.Message}");
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task CheckAccounts()
        {
            try
            {
                _logger.LogInformation("Starting account checks...");

                var customers = await _context.Customers.ToListAsync();

                var NGNCurrency = await _context.Currencies.Where(x => x.CurrencyCode == "NGN")
                    .FirstOrDefaultAsync();
                
                var GBPCurrency = await _context.Currencies.Where(x => x.CurrencyCode == "GBP")
                    .FirstOrDefaultAsync();
                
                var USDCurrency = await _context.Currencies.Where(x => x.CurrencyCode == "USD")
                    .FirstOrDefaultAsync();

                List<Currency> defaultCurrencies = new List<Currency>();
                defaultCurrencies.Add(NGNCurrency);
                defaultCurrencies.Add(GBPCurrency);
                defaultCurrencies.Add(USDCurrency);

                foreach (var customer in customers)
                {
                    //get all customer wallets
                    var currencies = await _context.Wallets
                        .Where(x => x.CustomerId == customer.Id)
                        .Select(x => x.Currency)
                        .ToListAsync();

                    _logger.LogInformation($"There are {currencies.Count} currencies.");

                    if (currencies.Count > 0)
                    {
                        foreach (var defaultCurrency in defaultCurrencies)
                        {
                            //if the available currencies does not contain one of the defaults...create the wallet
                            if (!currencies.Contains(defaultCurrency))
                            {
                                _logger.LogInformation($"This currency has not been added ===>{JsonSerializer.Serialize(defaultCurrency)}");
                                var newWallet = new Wallet()
                                {
                                    CustomerId = customer.Id,

                                    Currency = defaultCurrency
                                };

                                var addWallet = await _context.Wallets.AddAsync(newWallet);

                                if (addWallet != null)
                                {
                                    await _context.SaveChangesAsync();

                                    _logger.LogInformation("Wallet for the currency created.");
                                }
                            }
                        }
                    }

                }


            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private double CalcInterest(double principal, double rate, double time)
        {
            double interest;

            interest = (principal * rate * time) / 100;

            return interest;
        }

        private async Task<List<string>> GetAdminMailingList()
        {
            var mailingList = new List<string>();

            var users = await _userManager.Users
                .ToListAsync();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                foreach (var role in roles)
                {
                    if (role == "Admin" || role == "SuperAdmin")
                    {
                        mailingList.Add(user.Email);
                    }
                }
            }

            return mailingList;
        }
    }
}
