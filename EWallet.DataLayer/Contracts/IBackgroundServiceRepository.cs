
using System.Threading.Tasks;

namespace EWallet.DataLayer.Contracts
{
    public interface IBackgroundServiceRepository
    {
        Task AddInterest();

        Task CheckAccounts();
    }
}
