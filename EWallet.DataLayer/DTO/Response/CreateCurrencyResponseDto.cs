using System;

namespace EWallet.DataLayer.DTO.Response
{
    public class CreateCurrencyResponseDto
    {
        public Guid Id { get; set; }

        public DateTime DateCreated { get; set; }

        public string NameOfCurrency { get; set; }

        public string CurrencyCode { get; set; }

        public CreateCurrencyLogoResponseDto CurrencyLogo { get; set; }
    }
}
