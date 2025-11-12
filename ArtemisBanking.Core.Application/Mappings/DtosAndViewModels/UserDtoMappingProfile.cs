using ArtemisBanking.Core.Application.Dtos.User;
using ArtemisBanking.Core.Application.ViewModels.Users;
using AutoMapper;

namespace ArtemisBanking.Core.Application.Mappings.DtosAndViewModels;

public class UserDtoMappingProfile : Profile
{
    public UserDtoMappingProfile()
    {
        CreateMap<UserDto, UserViewModel>().ReverseMap();
    }
}