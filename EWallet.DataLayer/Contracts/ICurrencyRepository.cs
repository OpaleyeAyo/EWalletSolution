using EWallet.DataLayer.DTO.Request;
using EWallet.DataLayer.DTO.Response;
using EWallet.Entities.DbEntities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EWallet.DataLayer.Contracts
{
    public interface ICurrencyRepository
    {
        Task<GetCurrencyResponseDto> AddAsync(CreateCurrencyRequestDto model);

        Task<List<GetCurrencyResponseDto>> GetAllAsync();

        Task<bool> UploadLogoAsync(UploadPhotoRequestDto model, Guid currencyId);

        Task<bool> UpdateAsync(UpdateCurrencyRequestDto model, Guid currencyId);

        Task<GetCurrencyResponseDto> GetCurrencyByCode(string code);

        Task<GetCurrencyResponseDto> GetCurrencyById(Guid id);

        Task<Currency> GetByIdAsync(Guid id);
    }
}
