
namespace EWallet.DataLayer.DTO.Generic
{
    public class GenericEventDto<T> where T : class
    {
        public string EventType { get; set; }

        public T EventModel { get; set; }
    }
}
