using ArtemisBanking.Core.Application.Dtos.Login;
using ArtemisBanking.Core.Application.ViewModels.Login;
using AutoMapper;

namespace ArtemisBanking.Core.Application.Mappings.DtosAndViewModels;

public class LoginDtoMappingProfile :  Profile
{
    public LoginDtoMappingProfile()
    {
        
        CreateMap<LoginViewModel, LoginDto>().ReverseMap();
    }
    
}