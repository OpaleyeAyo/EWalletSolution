using EWallet.DataLayer.DTO.Request;
using System;
using System.Threading.Tasks;

namespace EWallet.DataLayer.Contracts
{
    public interface IPhotoRepository
    {
        Task<bool> UploadPhotoAsync(UploadPhotoRequestDto model, Guid userIdentityId);

        Task<bool> UploadLogoAsync(UploadPhotoRequestDto model, Guid currencyId);
    }
}
