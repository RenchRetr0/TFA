using AutoMapper;
using TFA.Storage.Entities;

namespace TFA.Storage.Mapping;

internal class ForumProfile : Profile
{
    public ForumProfile()
    {
        CreateMap<Forum, Domain.Models.Forum>()
            .ForMember(destination => destination.Id, source => source.MapFrom(f => f.ForumId));
    }
}
