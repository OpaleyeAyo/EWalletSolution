using AutoMapper;
using AutoMapper.QueryableExtensions;
using EWallet.DataLayer.Contracts;
using EWallet.DataLayer.Data;
using EWallet.DataLayer.DTO.Request;
using EWallet.DataLayer.DTO.Response;
using EWallet.Entities.DbEntities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EWallet.DataLayer.Implementations
{
    public class CurrencyRepository : ICurrencyRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IPhotoRepository _iPhotoRepository;

        public CurrencyRepository(IMapper mapper, AppDbContext context, IPhotoRepository iPhotoRepository)
        {
            _context = context;
            _mapper = mapper;
            _iPhotoRepository = iPhotoRepository;
        }

        public async Task<GetCurrencyResponseDto> AddAsync(CreateCurrencyRequestDto model)
        {
            try
            {
                GetCurrencyResponseDto response = new GetCurrencyResponseDto();

                Currency currencyExist = await GetByCodeAsync(model.CurrencyCode);

                if (currencyExist != null)
                {
                    return null;
                }

                Currency currency = new Currency()
                {
                    NameOfCurrency = model.NameOfCurrency,
                    CurrencyCode = model.CurrencyCode,
                };

                var newCurrency = await _context.Currencies.AddAsync(currency);

                if (newCurrency == null)
                {
                    return null;
                }

                await _context.SaveChangesAsync();

                if (model.Logo == null)
                {
                    return _mapper.Map<GetCurrencyResponseDto>(newCurrency.Entity);
                }

                UploadPhotoRequestDto uploadPhotoRequestDto = new UploadPhotoRequestDto()
                {
                    Photo = model.Logo 
                };

                bool addLogo = await _iPhotoRepository.UploadLogoAsync(uploadPhotoRequestDto, newCurrency.Entity.Id);

                if (!addLogo)
                {
                    return _mapper.Map<GetCurrencyResponseDto>(newCurrency.Entity);
                }

                await _context.SaveChangesAsync();

                return _mapper.Map<GetCurrencyResponseDto>(newCurrency.Entity);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> UploadLogoAsync(UploadPhotoRequestDto model, Guid currencyId)
        {
            try
            {
                var existingCurrency = await GetByIdAsync(currencyId);

                if (existingCurrency == null)
                {
                    return false;
                }

                bool addLogo = await _iPhotoRepository.UploadLogoAsync(model, existingCurrency.Id);

                if (!addLogo)
                {
                    return false;
                }

                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> UpdateAsync(UpdateCurrencyRequestDto model, Guid currencyId)
        {
            try
            {
                var currency = await GetByIdAsync(currencyId);

                if (currency == null)
                {
                    return false;
                }

                currency.CurrencyCode = model.CurrencyCode;
                currency.NameOfCurrency = model.NameOfCurrency;
                currency.DateUpdated = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<GetCurrencyResponseDto> GetCurrencyByCode(string code)
        {
            return await _context.Currencies
                    .Where(x => x.CurrencyCode == code)
                    .ProjectTo<GetCurrencyResponseDto>(_mapper.ConfigurationProvider)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();
        }

        public async Task<GetCurrencyResponseDto> GetCurrencyById(Guid id)
        {
            return await _context.Currencies
                    .Where(x => x.Id == id)
                    .ProjectTo<GetCurrencyResponseDto>(_mapper.ConfigurationProvider)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();
        }

        public async Task<List<GetCurrencyResponseDto>> GetAllAsync()
        {
            try
            {
                return await _context.Currencies
                    .AsNoTracking()
                    .ProjectTo<GetCurrencyResponseDto>(_mapper.ConfigurationProvider)
                    .ToListAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<Currency> GetByIdAsync(Guid id)
        {
            return await _context.Currencies
                    .Where(x => x.Id == id)
                    .FirstOrDefaultAsync();
        }

        private async Task<Currency> GetByCodeAsync(string currencyCode)
        {
            return await _context.Currencies
                    .Where(x => x.CurrencyCode == currencyCode)
                    .FirstOrDefaultAsync();
        }
    }
}
