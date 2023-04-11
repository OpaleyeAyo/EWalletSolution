using AutoMapper;
using AutoMapper.QueryableExtensions;
using EWallet.DataLayer.Contracts;
using EWallet.DataLayer.Data;
using EWallet.DataLayer.DTO.Generic;
using EWallet.DataLayer.DTO.Request;
using EWallet.DataLayer.DTO.Response;
using EWallet.Entities.DbEntities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EWallet.DataLayer.Implementations
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public CustomerRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<bool> ActivateAsync(Guid userId)
        {
            Customer customer = await GetCustomer(userId);

            if (customer == null)
            {
                return false;
            }

            customer.IsActive = true;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<GetCustomerResponseDto> AddAsync(Customer model)
        {
            var result = await _context.Customers.AddAsync(model);

            if (result != null)
            {
                await _context.SaveChangesAsync();

                return _mapper.Map<GetCustomerResponseDto>(result.Entity);
            }

            return new GetCustomerResponseDto();
        }

        public async Task<bool> DeactivateAsync(Guid userId)
        {
            Customer customer = await GetCustomer(userId);

            if (customer == null)
            {
                return false;
            }

            customer.IsActive = false;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var result = await _context.Customers
                .Where(x => x.Id == id).FirstOrDefaultAsync();

            if (result != null)
            {
                _context.Customers.Remove(result);

                await _context.SaveChangesAsync();

                return true;
            }

            return false;
        }

        public async Task<PagedResult<GetCustomerResponseDto>> GellAllAsync(CustomerParams customerParams)
        {
            var query = _context.Customers
                        .Include(x => x.Wallets)
                        .ProjectTo<GetCustomerResponseDto>(_mapper.ConfigurationProvider)
                        .AsNoTracking();

            return await PagedResult<GetCustomerResponseDto>.CreateAsync(query, customerParams.PageNumber, customerParams.PageSize);
        }

        public async Task<GetCustomerResponseDto> GetByEmailAsync(string emailAddress)
        {
            return await _context.Customers
                .Where(x => x.Email == emailAddress)
                .AsNoTracking()
                .ProjectTo<GetCustomerResponseDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task<GetCustomerResponseDto> GetByIdAsync(Guid id)
        {
            return await _context.Customers
                .Where(x => x.Id == id)
                .AsNoTracking()
                .ProjectTo<GetCustomerResponseDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task<GetCustomerResponseDto> GetByIdentityIdAsync(Guid identityId)
        {
            return await _context.Customers
                .Where(x => x.IdentityId == identityId)
                .AsNoTracking()
                .ProjectTo<GetCustomerResponseDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateAsync(UpdateCustomerRequestDto entity, Guid id)
        {
            var existingCustomer = await GetCustomer(id);

            if (existingCustomer != null)
            {
                //existingCustomer.City = entity.City;
                //existingCustomer.Country = entity.Country;
                //existingCustomer.State = entity.State;
                //existingCustomer.PhoneNumber = entity.PhoneNumber;
                //existingCustomer.DateUpdated = DateTime.UtcNow;

                var result = await _context.SaveChangesAsync();

                return result > 0;
            }

            return false;
        }

        private async Task<Customer> GetCustomer(Guid id)
        {
            return await _context.Customers
                .Where(x => x.Id == id)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }
    }
}
