using System.Collections.Generic;

namespace EWallet.DataLayer.DTO.Errors
{
    public class ErrorResponse
    {
        public List<ErrorModel> Errors { get; set; } = new List<ErrorModel>();
    }
}
