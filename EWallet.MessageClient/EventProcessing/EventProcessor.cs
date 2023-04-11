using EWallet.DataLayer.Contracts;
using EWallet.DataLayer.Data;
using EWallet.DataLayer.DTO.BgServiceModel;
using EWallet.DataLayer.DTO.Generic;
using EWallet.DataLayer.DTO.Response;
using EWallet.Entities.DbEntities;
using FluentEmail.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EWallet.MessageClient.EventProcessing
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IServiceScopeFactory _scopeFactory;
        
        private readonly ILogger<EventProcessor> _logger;
        
        public EventProcessor(IServiceScopeFactory scopeFactory, ILogger<EventProcessor> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        private string DetermineEvent(string notificationMessage)
        {
            _logger.LogInformation("Determining event type");

            var eventType = JsonConvert.DeserializeObject<dynamic>(notificationMessage)!;

            string eventString = eventType.EventType;

            if (eventString == AsyncEventType.CreateWalletsForCustomer)
            {
                _logger.LogInformation("CREATE_USER_WALLET event detected");
                return AsyncEventType.CreateWalletsForCustomer;
            }
            else if (eventString == AsyncEventType.SendInterestAdditionNotification)
            {
                _logger.LogInformation("SIMPLE_INTEREST_ADDITION_NOTIFICATION event detected");
                return AsyncEventType.SendInterestAdditionNotification;
            }
            else if (eventString == AsyncEventType.CreateNewSettlementAccount)
            {
                _logger.LogInformation("CREATE_SETTLEMENT_ACCOUNT event detected");
                return AsyncEventType.CreateNewSettlementAccount;
            }
            else
            {
                _logger.LogInformation("Could not determine event type.");
                return AsyncEventType.Undertermined;
            }
        }

        public async Task ProcessEvent(string message)
        {
            var eventType = DetermineEvent(message);

            _logger.LogInformation($"Event type to be processed---- {eventType}");

            if (eventType == AsyncEventType.CreateWalletsForCustomer)
            {
                var createWallet = await CreateWalletAsync(message);

                if (createWallet)
                {
                    _logger.LogInformation($"Wallets created successfully.");

                    return;
                }

                _logger.LogInformation($"Failed. Cannot create wallets.");

                return;
            }
            else if (eventType == AsyncEventType.CreateNewSettlementAccount)
            {
                var createAccount = await CreateSettlementAccountAsync(message);

                if (createAccount)
                {
                    _logger.LogInformation($"Settlement account created successfully.");

                    return;
                }

                _logger.LogInformation($"Failed. Cannot create settlement account.");

                return;
            }
            else if (eventType == AsyncEventType.SendInterestAdditionNotification)
            {
                await SendAdminInterestCreditEmailNotification(message);

                var sendMail = await SendInterestCreditEmailNotification(message);

                if (sendMail)
                {
                    _logger.LogInformation($"Interest credit notification mail sent successfully.");

                    return;
                }

                _logger.LogInformation($"Failed. Cannot send notification for interest credit.");

                return;
            }
            else
            {
                _logger.LogInformation("Undetermined event type...");
            }

            return;
        }

        private async Task<bool> SendInterestCreditEmailNotification(string eventString)
        {
            var eventModel = JsonConvert.DeserializeObject<GenericEventDto<InterestNotificationModel>>(eventString);

            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var mailer = scope.ServiceProvider.GetRequiredService<IFluentEmail>();

                    string directory = $"{Directory.GetCurrentDirectory()}/wwwroot/Emails/SendInterestAlert.cshtml";

                    _logger.LogInformation($"Email directory ====> : {directory}");

                    var email = mailer
                        .To(eventModel.EventModel.CustomerEmail, eventModel.EventModel.Name)
                        .Subject("Credit Alert")
                        .Tag(Utility.Helper.HelperMethods.GenerateRandom(6))
                        .UsingTemplateFromFile(directory, eventModel.EventModel);

                    var sendEmail = await email.SendAsync();

                    if (sendEmail.Successful)
                    {
                        _logger.LogInformation("Email sent");
                        return true;
                    }

                    _logger.LogInformation("Email not sent");

                    return false;
                }
            }
            catch (Exception ex)
            {

                throw;
            }

        }
        
        private async Task<bool> CreateSettlementAccountAsync(string eventString)
        {
            var eventModel = JsonConvert.DeserializeObject<GenericEventDto<GetCurrencyResponseDto>>(eventString);
            
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    _logger.LogInformation($"Start.... creating settlement account for {eventModel.EventModel.NameOfCurrency}");

                    var _context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    Currency currency = await _context.Currencies
                        .Where(x => x.CurrencyCode == eventModel.EventModel.CurrencyCode)
                        .FirstOrDefaultAsync();

                    if (currency == null)
                    {
                        _logger.LogError("Failed to create settlement account. Currency type is null.");
                        return false;
                    }

                    var settlementAccount = new SettlementAccount() 
                    {
                        AccountName = eventModel.EventModel.NameOfCurrency,
                        AccountBalance = 30000000000000,
                        Currency = currency,
                        CurrencyId = currency.Id
                    };

                    var newAccount = await _context.SettlementAccounts.AddAsync(settlementAccount);

                    if (newAccount != null)
                    {
                        await _context.SaveChangesAsync();

                        _logger.LogInformation("Settlement account created.");

                        return true;
                    }

                    _logger.LogError("Failed to create settlement account. An error occurred.");

                    return false;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        
        private async Task<bool> CreateWalletAsync(string eventString)
        {
            var eventModel = JsonConvert.DeserializeObject<GenericEventDto<GetCustomerResponseDto>>(eventString);

            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var _context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    Customer customer = await _context.Customers
                        .Where(x => x.Id == eventModel.EventModel.Id)
                        .FirstOrDefaultAsync();

                    Currency nairaCurrency = await _context.Currencies
                        .Where(x => x.CurrencyCode == CurrencyCode.NigerianNaira)
                        .FirstOrDefaultAsync();

                    Currency usdCurrency = await _context.Currencies
                        .Where(x => x.CurrencyCode == CurrencyCode.USDollar)
                        .FirstOrDefaultAsync();

                    Currency gbpCurrency = await _context.Currencies
                        .Where(x => x.CurrencyCode == CurrencyCode.BritishPound)
                        .FirstOrDefaultAsync();

                    if (customer != null && nairaCurrency != null && usdCurrency != null && gbpCurrency != null)
                    {
                        List<Wallet> wallets = new List<Wallet>()
                        {
                            new Wallet
                            {
                                AccountBalance = 0,
                                Currency = nairaCurrency,
                                CustomerId = customer.Id,
                                Customer = customer
                            },
                            new Wallet
                            {
                                AccountBalance = 0,
                                Currency = usdCurrency,
                                CustomerId = customer.Id,
                                Customer = customer
                            },
                            new Wallet
                            {
                                AccountBalance = 0,
                                Currency = gbpCurrency,
                                CustomerId = customer.Id,
                                Customer = customer
                            }
                        };

                        foreach (var wallet in wallets)
                        {
                            await _context.Wallets.AddAsync(wallet);

                            await _context.SaveChangesAsync();

                            _logger.LogInformation($"{wallet.Currency.CurrencyCode} Wallet added.");
                        }

                        return true;
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex.StackTrace}");

                throw;
            }
        }
        
        private async Task SendAdminInterestCreditEmailNotification(string eventString)
        {
            var eventModel = JsonConvert.DeserializeObject<GenericEventDto<InterestNotificationModel>>(eventString);

            try
            {
                _logger.LogInformation("Sending admin email notifications.");

                var reciepients = eventModel.EventModel.ListOfAdminEmail;

                string directory = $"{Directory.GetCurrentDirectory()}/wwwroot/Emails/SendAdminInterestAlert.cshtml";

                _logger.LogInformation($"Email directory ====> : {directory}");
                

                var emailModel = new AdminNotificationModel()
                {
                    Name = "Admin",
                    Date = eventModel.EventModel.DateOfTransaction
                };

                using (var scope = _scopeFactory.CreateScope())
                {
                    var factory = scope.ServiceProvider.GetRequiredService<IFluentEmailFactory>();

                    foreach (var reciepient in reciepients)
                    {
                        _logger.LogInformation($"Email reciepient ====> : {reciepient}");

                        var email = factory
                            .Create()
                            //.To(reciepient.ToString())
                            .To(reciepient)
                            .Subject("Simple Interest Credit Notification")
                            .Tag(Utility.Helper.HelperMethods.GenerateRandom(6))
                            .UsingTemplateFromFile(directory, emailModel);

                        var sendEmail = await email.SendAsync();

                        if (sendEmail.Successful)
                        {
                            _logger.LogInformation($"Admin Email notification sent to ====> : {reciepient}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
