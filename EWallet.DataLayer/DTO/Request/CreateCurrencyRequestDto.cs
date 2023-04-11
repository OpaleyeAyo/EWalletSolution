using EWallet.DataLayer.Validators;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace EWallet.DataLayer.DTO.Request
{
    public class CreateCurrencyRequestDto
    {
        [Required]
        public string NameOfCurrency { get; set; }

        [Required]
        public string CurrencyCode { get; set; }

        [DataType(DataType.Upload)]
        [MaxFileSize(5 * 1024 * 1024)]
        [AllowedExtensions(new string[] { ".jpg", ".png" })]
        public IFormFile Logo { get; set; }
    }
}
