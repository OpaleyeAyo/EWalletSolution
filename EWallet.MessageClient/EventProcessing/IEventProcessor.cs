using System.Threading.Tasks;

namespace EWallet.MessageClient.EventProcessing
{
    public interface IEventProcessor
    {
        Task ProcessEvent(string message);

        //Task<bool> CreateWalletAsync(string eventString);
    }
}
