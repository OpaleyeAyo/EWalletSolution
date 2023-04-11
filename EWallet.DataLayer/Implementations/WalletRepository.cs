using AutoMapper;
using AutoMapper.QueryableExtensions;
using EWallet.DataLayer.Contracts;
using EWallet.DataLayer.Data;
using EWallet.DataLayer.DTO.Generic;
using EWallet.DataLayer.DTO.Request;
using EWallet.DataLayer.DTO.Response;
using EWallet.Entities.DbEntities;
using EWallet.Entities.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EWallet.DataLayer.Implementations
{
    public class WalletRepository : IWalletRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<WalletRepository> _logger;
        private readonly IMapper _mapper;
        private readonly ICustomerRepository _customerRepository;

        public WalletRepository(AppDbContext context, IMapper mapper,
            ICustomerRepository customerRepository,
            ILogger<WalletRepository> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _customerRepository = customerRepository;
        }

        public async Task<PaymentDepositResponseDto> MakeDepositAsync(PaymentDepositRequestDto model, Guid userIdentityId)
        {
            try
            {
                Transaction transaction = new Transaction();

                var validateResponse = await ValidateAsync(userIdentityId, model.AccountNumber);

                Wallet wallet = validateResponse.Wallet;
                SettlementAccount settlementAccount = validateResponse.SettlementAccount;
                Customer customer = validateResponse.Customer;
                Currency currency = validateResponse.Currency;

                //make the changes in the two account balances
                decimal newWalletBal = wallet.AccountBalance + model.Amount;
                wallet.AccountBalance = newWalletBal;
                wallet.DateUpdated = DateTime.UtcNow;
                var updateWallet = await _context.SaveChangesAsync();

                decimal newSettlementBal = settlementAccount.AccountBalance - model.Amount;
                settlementAccount.AccountBalance = newSettlementBal;
                settlementAccount.DateUpdated = DateTime.UtcNow;
                var updateSettlementAccount = await _context.SaveChangesAsync();

                if (updateWallet >= 0 && updateSettlementAccount >= 0)
                {
                    transaction.TypeOfTransaction = TransactionType.DEPOSIT;
                    transaction.CurrencyCode = currency.CurrencyCode;
                    transaction.TransactionSourceAccount = settlementAccount.AccountNumber;
                    transaction.TransactionDestinationAccount = wallet.AccountNumber;
                    transaction.Amount = model.Amount;
                    transaction.TransactionStatus = TransactionStatus.Success;
                    transaction.CustomerIdentityId = customer.IdentityId;
                    transaction.WalletId = wallet.Id;

                    _logger.LogInformation("Transaction successful.");
                    
                } else
                {
                    transaction.TypeOfTransaction = TransactionType.DEPOSIT;
                    transaction.CurrencyCode = currency.CurrencyCode;
                    transaction.TransactionSourceAccount = settlementAccount.AccountNumber;
                    transaction.TransactionDestinationAccount = wallet.AccountNumber;
                    transaction.Amount = model.Amount;
                    transaction.TransactionStatus = TransactionStatus.Failed;
                    transaction.CustomerIdentityId = customer.IdentityId;
                    transaction.WalletId = wallet.Id;

                    _logger.LogError("Transaction failed.");
                }

                var newTransac = await _context.Transactions.AddAsync(transaction);

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Transaction with reference number of {newTransac.Entity.TransactionReference} saved to the database.");

                return _mapper.Map<PaymentDepositResponseDto>(newTransac.Entity);

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<GetWalletResponseDto> GetByIdAsync(Guid walletId)
        {
            try
            {
                return await _context.Wallets
                    .Where(x => x.Id == walletId)
                    .AsNoTracking()
                    .ProjectTo<GetWalletResponseDto>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<GetWalletResponseDto> GetByAccountNumberAsync(string accountNumber)
        {
            try
            {
                return await _context.Wallets
                    .Where(x => x.AccountNumber == accountNumber)
                    .AsNoTracking()
                    .ProjectTo<GetWalletResponseDto>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<PagedResult<GetWalletResponseDto>> GetUserWalletsAsync(WalletParams walletParams, Guid userIdentityId)
        {
            try
            {
                var user = await _context.Customers
                    .Where(x => x.IdentityId == userIdentityId)
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    return null;
                }

                var query = _context.Wallets
                    .Where(x => x.CustomerId == user.Id)
                    .AsNoTracking()
                    .ProjectTo<GetWalletResponseDto>(_mapper.ConfigurationProvider);

                return await PagedResult<GetWalletResponseDto>.CreateAsync(query, walletParams.PageNumber, walletParams.PageSize);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<PaymentWithdrawalResponseDto> MakeWithdrawalAsync(PaymentWithdrawalRequestDto model, Guid userIdentityId)
        {
            try
            {
                var transaction = new Transaction();

                var validateResponse = await ValidateAsync(userIdentityId, model.AccountNumber);

                Wallet wallet = validateResponse.Wallet;
                SettlementAccount settlementAccount = validateResponse.SettlementAccount;
                Customer customer = validateResponse.Customer;

                if (wallet.AccountBalance < model.Amount)
                {
                    _logger.LogError("Insufficient Balance");

                    return null;
                }

                //make the changes in the two account balances if they are valid
                wallet.AccountBalance -= model.Amount;
                wallet.DateUpdated = DateTime.Now;
                int updateWallet = await _context.SaveChangesAsync();

                settlementAccount.AccountBalance += model.Amount;
                settlementAccount.DateUpdated = DateTime.Now;
                int updateSettlementAccount = await _context.SaveChangesAsync();

                //check if the changes occur in the two accounts
                if (updateWallet >= 0 && updateSettlementAccount >= 0)
                {
                    //Transaction successful
                    transaction.TransactionStatus = TransactionStatus.Success;
                    _logger.LogInformation("Transaction successful");
                }
                else
                {
                    //Transaction not successful
                    transaction.TransactionStatus = TransactionStatus.Failed;
                    _logger.LogError("Transaction failed");
                }

                transaction.TypeOfTransaction = TransactionType.WITHDRAWAL;
                transaction.TransactionSourceAccount = wallet.AccountNumber;
                transaction.TransactionDestinationAccount = settlementAccount.AccountNumber;
                transaction.Amount = model.Amount;
                transaction.CustomerIdentityId = customer.IdentityId;
                transaction.WalletId = wallet.Id;
                
                var newTransac = await _context.Transactions.AddAsync(transaction);

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Transaction with reference number of {newTransac.Entity.TransactionReference} saved to the database.");

                return _mapper.Map<PaymentWithdrawalResponseDto>(newTransac.Entity);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<PagedResult<GetWalletResponseDto>> GetAllAsync(WalletParams walletParams)
        {
            try
            {
                var query = _context.Wallets
                    .AsNoTracking()
                    .ProjectTo<GetWalletResponseDto>(_mapper.ConfigurationProvider);
                
                return await PagedResult<GetWalletResponseDto>.CreateAsync(query, walletParams.PageNumber, walletParams.PageSize);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<GetWalletResponseDto> CreateNewWalletAsync(CreateWalletRequestDto model, Guid identityId)
        {
            try
            {
                Customer user = await _context.Customers
                    .Where(x => x.IdentityId == identityId)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    return null;
                }

                //check if the currency type exist
                Currency currency = await _context.Currencies
                    .Where(x => x.CurrencyCode == model.CurrencyCode)
                    .FirstOrDefaultAsync();

                if (currency == null)
                {
                    return null;
                }

                //check if the user has a wallet with currency type
                Wallet walletExist = await _context.Wallets
                    .Where(x => x.CustomerId == user.Id && x.Currency.Id == currency.Id)
                    .FirstOrDefaultAsync();

                if (walletExist != null)
                {
                    return null;
                }

                Wallet newWallet = new Wallet()
                {
                    AccountBalance = 0,
                    Currency = currency,

                    CustomerId = user.Id
                };

                var createWallet = await _context.Wallets.AddAsync(newWallet);

                if (createWallet != null)
                {
                    await _context.SaveChangesAsync();

                    return _mapper.Map<GetWalletResponseDto>(createWallet.Entity);
                }

                return null;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async Task<ValidateTransactionProcessResponseDto> ValidateAsync(Guid identityId, string accountNumber)
        {
            var customer = _mapper.Map<Customer>(await _customerRepository.GetByIdentityIdAsync(identityId));

            if (customer == null)
            {
                _logger.LogError($"Customer with the identity id of {identityId} does not exist.");

                return null;
            }

            var wallet = await _context.Wallets
                .Where(x => x.AccountNumber == accountNumber && x.CustomerId == customer.Id)
                .FirstOrDefaultAsync();

            if (wallet == null)
            {
                _logger.LogError($"Wallet with the account number of {accountNumber} does not exist.");

                return null;
            }

            _logger.LogInformation($"Wallet with the account number of {accountNumber} retrieved");

            //check wallet currency type
            var currency = await _context.Wallets
                .Where(x => x.AccountNumber == accountNumber)
                .Select(x => x.Currency)
                .FirstOrDefaultAsync();

            if (currency == null)
            {
                _logger.LogError($"Currency type for the wallet with the account number of {accountNumber} not found.");

                return null;
            }

            _logger.LogInformation($"Currency type for the wallet with the account number of {accountNumber} is {currency.NameOfCurrency}");


            //Get Settlement account for the currency type
            var settlementAccount = await _context.SettlementAccounts
                .Where(x => x.CurrencyId == currency.Id)
                .FirstOrDefaultAsync();

            if (settlementAccount == null)
            {
                _logger.LogError($"Settlement account type for the currency with the id of {currency.Id} not found.");

                return null;
            }

            var currencyType = await _context.SettlementAccounts
                .Where(x => x.CurrencyId == currency.Id)
                .Select(x => x.Currency)
                .FirstOrDefaultAsync();

            _logger.LogInformation($"Settlement account type for the currency with the id of {currency.Id} retrieved.");

            _logger.LogInformation($"Currency Code: {currency.CurrencyCode}. Settlement account currency code: {currencyType.CurrencyCode}");

            var response = new ValidateTransactionProcessResponseDto()
            {
                Wallet = wallet,
                SettlementAccount = settlementAccount,
                Customer = customer,
                Currency = currency
            };

            return response;
        }

        public async Task<PagedResult<GetWalletResponseDto>> GetCustomerWalletsAsync(WalletParams walletParams, Guid customerId)
        {
            try
            {
                var user = await _context.Customers
                    .Where(x => x.Id == customerId)
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    return null;
                }

                var query = _context.Wallets
                    .Where(x => x.CustomerId == user.Id)
                    .AsNoTracking()
                    .ProjectTo<GetWalletResponseDto>(_mapper.ConfigurationProvider);

                return await PagedResult<GetWalletResponseDto>.CreateAsync(query, walletParams.PageNumber, walletParams.PageSize);
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
