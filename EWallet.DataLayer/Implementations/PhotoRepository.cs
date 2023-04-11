using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using EWallet.DataLayer.Contracts;
using EWallet.DataLayer.Data;
using EWallet.DataLayer.DTO.Request;
using EWallet.Entities.DbEntities;
using EWallet.Utility.Configurations;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EWallet.DataLayer.Implementations
{
    public class PhotoRepository : IPhotoRepository
    {
        private readonly AppDbContext _context;
        private readonly Cloudinary _cloudinary;

        public PhotoRepository(AppDbContext context, IOptions<CloudinarySettings> config)
        {
            _context = context;

            var account = new Account(config.Value.CloudName, config.Value.ApiKey, config.Value.ApiSecret);

            _cloudinary = new Cloudinary(account);
        }

        public async Task<bool> UploadLogoAsync(UploadPhotoRequestDto model, Guid currencyId)
        {
            ImageUploadResult result;

            var currency = await _context.Currencies
                .Where(x => x.Id == currencyId)
                .FirstOrDefaultAsync();

            if (currency == null)
            {
                return false;
            }

            //check if logo exist
            CurrencyLogo existingLogo = await _context.Currencies
                .Where(x => x.Id == currencyId)
                .Select(x => x.CurrencyLogo)
                .FirstOrDefaultAsync();

            if (existingLogo == null)
            {
                result = await AddPhotoAsync(model.Photo);

                if (result.Error != null)
                {
                    return false;
                }

                var logo = new CurrencyLogo()
                {
                    Url = result.SecureUrl.AbsoluteUri,
                    PublicId = result.PublicId,

                    CurrencyId = currency.Id
                };

                var addLogo = await _context.CurrencyLogos.AddAsync(logo);

                if (addLogo != null)
                {
                    await _context.SaveChangesAsync();

                    return true;
                }

                return false;
            }

            result = await AddPhotoAsync(model.Photo);

            if (result.Error != null)
            {
                return false;
            }

            existingLogo.Url = result.SecureUrl.AbsoluteUri;
            existingLogo.PublicId = result.PublicId;
            existingLogo.DateUpdated = DateTime.UtcNow;

            var updateDb = await _context.SaveChangesAsync();

            if (updateDb >= 0)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> UploadPhotoAsync(UploadPhotoRequestDto model, Guid userIdentityId)
        {
            ImageUploadResult result;

            var user = await _context.Customers
                .Where(x => x.IdentityId == userIdentityId)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return false;
            }

            //check if photo exist
            var photoExist = await _context.Customers
                .Where(x => x.IdentityId == userIdentityId)
                .Select(x => x.ProfilePhoto)
                .FirstOrDefaultAsync();

            if (photoExist == null)
            {
                result = await AddPhotoAsync(model.Photo);

                if (result.Error != null)
                {
                    return false;
                }

                var profilePhoto = new ProfilePhoto()
                {
                    Url = result.SecureUrl.AbsoluteUri,
                    PublicId = result.PublicId,

                    CustomerId = user.Id,
                    Customer = user
                };

                var addPhoto = await _context.ProfilePhotos.AddAsync(profilePhoto);

                if (addPhoto != null)
                {
                    await _context.SaveChangesAsync();

                    return true;
                }

                return false;
            }

            result = await AddPhotoAsync(model.Photo);

            if (result.Error != null)
            {
                return false;
            }

            photoExist.PublicId = result.PublicId;
            photoExist.Url = result.SecureUrl.AbsoluteUri;
            photoExist.DateUpdated = DateTime.UtcNow;

            var updateDb = await _context.SaveChangesAsync();

            if (updateDb >= 0)
            {
                return true;
            }

            return false;
        }

        private async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
        {
            var uploadResult = new ImageUploadResult();

            if (file.Length > 0)
            {
                using var stream = file.OpenReadStream();

                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file.FileName, stream),
                    //Transformation = new Transformation().Height(400).Width(400).Crop("fill"),
                };

                uploadResult = await _cloudinary.UploadAsync(uploadParams);
            }

            return uploadResult;
        }

        private async Task<DeletionResult> DeletePhotoAsync(string publicId)
        {
            var deletionParams = new DeletionParams(publicId);

            var result = await _cloudinary.DestroyAsync(deletionParams);

            return result;
        }
    }
}
