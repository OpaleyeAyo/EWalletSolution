using EWallet.DataLayer.DTO.Errors;
using System;

namespace EWallet.DataLayer.DTO.Generic
{
    public class Result<T>
    {
        public T Data { get; set; }
        public Error Error { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public DateTime ResponseTime { get; set; } = DateTime.UtcNow;
    }
}
