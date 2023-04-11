using EWallet.DataLayer.DTO.Generic;
using EWallet.DataLayer.DTO.Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EWallet.DataLayer.Contracts
{
    public interface ITransactionRepository
    {
        Task<PagedResult<GetTransactionResponseDto>> GetAllAsync(TransactionParams transactionParams);

        List<GetTransactionResponseDto> GetAll();

        Task<GetTransactionResponseDto> GetAsync(Guid transactionId);

        byte[] ExportToCsv();

        Task<PagedResult<GetTransactionResponseDto>> GetUserTransactionsAsync(TransactionParams transactionParams, Guid userIdentityId);
    }
}
