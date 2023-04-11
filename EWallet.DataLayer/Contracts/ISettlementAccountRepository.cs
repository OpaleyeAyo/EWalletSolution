using EWallet.DataLayer.DTO.Generic;
using EWallet.DataLayer.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EWallet.DataLayer.Contracts
{
    public interface ISettlementAccountRepository
    {
        Task<PagedResult<GetSettlementAccountResponseDto>> GetAllAsync(SettlementAccountParams settlementAccountparams);

        Task<GetSettlementAccountResponseDto> GetAsync(Guid accountId);
    }
}
