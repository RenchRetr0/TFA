using AutoMapper;

namespace TFA.Storage.Mapping;

internal class TopicProfile : Profile
{
    public TopicProfile()
    {
        CreateMap<Topic, Domain.Models.Topic>()
            .ForMember(destination => destination.Id, source => source.MapFrom(t => t.ForumId));
    }
}
