using AutoMapper;
using TFA.Domain.UseCase.SignIn;
using TFA.Storage.Entities;

namespace TFA.Storage.Mapping;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, RecognizedUser>();
    }
}
