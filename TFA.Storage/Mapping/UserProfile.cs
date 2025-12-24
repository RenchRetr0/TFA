using AutoMapper;
using TFA.Domain.UseCase.SignIn;

namespace TFA.Storage.Mapping;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, RecognizedUser>();
    }
}
