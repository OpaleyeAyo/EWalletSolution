using AutoMapper;
using EWallet.DataLayer.DTO.Response;
using EWallet.Entities.DbEntities;

namespace EWallet.DataLayer.AutoMapper
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<CurrencyLogo, GetCurrencyLogoResponseDto>()
                .ReverseMap();

            CreateMap<Customer, GetCustomerResponseDto>()
                .ReverseMap();
            
            CreateMap<Currency, GetCurrencyResponseDto>()
                .ReverseMap();

            CreateMap<Transaction, PaymentDepositResponseDto>()
                .ReverseMap();
            
            CreateMap<Wallet, BgServiceWalletResponseDto>()
                .ReverseMap();

            CreateMap<Transaction, GetTransactionResponseDto>()
                .ReverseMap();

            CreateMap<Transaction, PaymentWithdrawalResponseDto>()
                .ReverseMap();
            
            CreateMap<SettlementAccount, GetSettlementAccountResponseDto>()
                .ForMember(dest => dest.CurrencyCode, opt => opt 
                .MapFrom(src => src.Currency.CurrencyCode))
                .ReverseMap();

            CreateMap<Wallet, GetWalletResponseDto>()
                .ForMember(dest => dest.CurrencyCode, opt => opt
                .MapFrom( src => src.Currency.CurrencyCode))
                .ReverseMap();

            /*
             *CreateMap<AppUser, MemberResponseDTO>()
            .ForMember(dest => dest.PhotoUrl, opt => opt
            .MapFrom(src => src.Photos
            .Where(x => x.IsMain == true)
            .FirstOrDefault().Url))
            .ForMember(dest => dest.Age, opt => opt
            .MapFrom(src => src.DateOfBirth.CalculateAge()))
            .ReverseMap();
             */
        }
    }
}
