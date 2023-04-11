using EWallet.DataLayer.DTO.Generic;
using EWallet.DataLayer.DTO.Request;
using EWallet.DataLayer.DTO.Response;
using System;
using System.Threading.Tasks;

namespace EWallet.DataLayer.Contracts
{
    public interface IWalletRepository
    {
        Task<PaymentDepositResponseDto> MakeDepositAsync(PaymentDepositRequestDto model, Guid userIdentityId);  

        Task<PaymentWithdrawalResponseDto> MakeWithdrawalAsync(PaymentWithdrawalRequestDto model, Guid userIdentityId);

        Task<GetWalletResponseDto> GetByIdAsync(Guid walletId);

        Task<PagedResult<GetWalletResponseDto>> GetAllAsync(WalletParams walletParams);

        Task<GetWalletResponseDto> GetByAccountNumberAsync(string accountNumber);

        Task<PagedResult<GetWalletResponseDto>> GetUserWalletsAsync(WalletParams walletParams, Guid userIdentityId);
        
        Task<PagedResult<GetWalletResponseDto>> GetCustomerWalletsAsync(WalletParams walletParams, Guid customerId);

        Task<GetWalletResponseDto> CreateNewWalletAsync(CreateWalletRequestDto model, Guid identityId);
    }
}
