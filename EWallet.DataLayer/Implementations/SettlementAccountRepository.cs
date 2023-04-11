using AutoMapper;
using AutoMapper.QueryableExtensions;
using EWallet.DataLayer.Contracts;
using EWallet.DataLayer.Data;
using EWallet.DataLayer.DTO.Generic;
using EWallet.DataLayer.DTO.Response;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EWallet.DataLayer.Implementations
{
    public class SettlementAccountRepository : ISettlementAccountRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public SettlementAccountRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PagedResult<GetSettlementAccountResponseDto>> GetAllAsync(SettlementAccountParams settlementAccountParams)
        {
            try
            {
                var query = _context.SettlementAccounts
                    .AsNoTracking()
                    .ProjectTo<GetSettlementAccountResponseDto>(_mapper.ConfigurationProvider);

                return await PagedResult<GetSettlementAccountResponseDto>.CreateAsync(query, settlementAccountParams.PageNumber, settlementAccountParams.PageSize);

            }
            catch (Exception ex)
            {

                throw ex;
            }
           
        }

        public async Task<GetSettlementAccountResponseDto> GetAsync(Guid accountId)
        {
            try
            {
                return await _context.SettlementAccounts
                .Where(x => x.Id == accountId)
                .ProjectTo<GetSettlementAccountResponseDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
