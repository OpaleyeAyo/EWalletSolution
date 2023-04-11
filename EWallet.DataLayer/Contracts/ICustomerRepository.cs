using EWallet.DataLayer.DTO.Generic;
using EWallet.DataLayer.DTO.Request;
using EWallet.DataLayer.DTO.Response;
using EWallet.Entities.DbEntities;
using System;
using System.Threading.Tasks;

namespace EWallet.DataLayer.Contracts
{
    public interface ICustomerRepository
    {
        Task<PagedResult<GetCustomerResponseDto>> GellAllAsync(CustomerParams customerParams);

        Task<GetCustomerResponseDto> GetByIdAsync(Guid id);

        Task<GetCustomerResponseDto> GetByEmailAsync(string emailAddress);

        Task<GetCustomerResponseDto> GetByIdentityIdAsync(Guid identityId);

        Task<bool> ActivateAsync(Guid userId);

        Task<bool> DeactivateAsync(Guid userId);

        Task<GetCustomerResponseDto> AddAsync(Customer model);

        Task<bool> DeleteAsync(Guid id);

        //Task<bool> UpdateAsync(UpdateCustomerRequestDto entity, Guid id);
    }
}
