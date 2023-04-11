using System;

namespace EWallet.DataLayer.DTO.Response
{
    public class GetCurrencyResponseDto
    {
        public Guid Id { get; set; }

        public DateTime DateCreated { get; set; }

        public string NameOfCurrency { get; set; }

        public string CurrencyCode { get; set; }

        public GetCurrencyLogoResponseDto CurrencyLogo { get; set; }
    }
}
