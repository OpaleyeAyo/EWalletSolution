
namespace EWallet.API.AsyncDataTransfer
{
    public interface IMessageBusClient
    {
        void PubMessage<T>(T model);
    }
}
