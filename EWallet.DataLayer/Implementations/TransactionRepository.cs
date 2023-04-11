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
    public class TransactionRepository : ITransactionRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public TransactionRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PagedResult<GetTransactionResponseDto>> GetAllAsync(TransactionParams transactionParams)
        {
            try
            {
                var query = _context.Transactions
                    .AsNoTracking()
                    .ProjectTo<GetTransactionResponseDto>(_mapper.ConfigurationProvider);

                return await PagedResult<GetTransactionResponseDto>.CreateAsync(query, transactionParams.PageNumber, transactionParams.PageSize);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public byte[] ExportToCsv()
        {
            try
            {
                var transactions = _context.Transactions
                    .AsNoTracking()
                    .ProjectTo<GetTransactionResponseDto>(_mapper.ConfigurationProvider)
                    .ToList();

                        string[] colmunNames = new string[]
                        {
                            "Id", "TransactionReference", "TransactionSourceAccount",
                            "CurrencyCode", "TransactionDestinationAccount" , "Amount", "TransactionDate",
                            "TypeOfTransaction", "TransactionStatus"
                        };

                string csv = string.Empty;

                foreach (string colmunName in colmunNames)
                {
                    csv += colmunName + ',';
                }

                csv += "\r\n";

                foreach (var transaction in transactions)
                {
                    csv += transaction.Id.ToString().Replace(",", ";") + ',';
                    csv += transaction.TransactionReference.Replace(",", ";") + ',';
                    csv += transaction.TransactionSourceAccount.Replace(",", ";") + ',';
                    csv += transaction.CurrencyCode.Replace(",", ";") + ',';
                    csv += transaction.TransactionDestinationAccount.Replace(",", ";") + ',';
                    csv += transaction.Amount.ToString().Replace(",", ";") + ',';
                    csv += transaction.TransactionDate.ToString().Replace(",", ";") + ',';
                    csv += transaction.TypeOfTransaction.ToString().Replace(",", ";") + ',';
                    csv += transaction.TransactionStatus.ToString().Replace(",", ";") + ',';

                    csv += "\r\n";
                }

                byte[] bytes = Encoding.ASCII.GetBytes(csv);

                return bytes;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<GetTransactionResponseDto> GetAsync(Guid transactionId)
        {
            try
            {
                return await _context.Transactions
                    .Where(x => x.Id == transactionId)
                    .AsNoTracking()
                    .ProjectTo<GetTransactionResponseDto>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<PagedResult<GetTransactionResponseDto>> GetUserTransactionsAsync(TransactionParams transactionParams, Guid userIdentityId)
        {
            try
            {
                var user = await _context.Customers
                    .Where(x => x.IdentityId == userIdentityId)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    return null;
                }


                var query = _context.Transactions
                    .Where(x => x.CustomerIdentityId == user.IdentityId)
                    .AsNoTracking()
                    .ProjectTo<GetTransactionResponseDto>(_mapper.ConfigurationProvider);

                return await PagedResult<GetTransactionResponseDto>.CreateAsync(query, transactionParams.PageNumber, transactionParams.PageSize);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public List<GetTransactionResponseDto> GetAll()
        {
            return _context.Transactions
                    .AsNoTracking()
                    .ProjectTo<GetTransactionResponseDto>(_mapper.ConfigurationProvider)
                    .ToList();
        }
    }
}
