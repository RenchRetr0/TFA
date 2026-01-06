using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TFA.Domain.UseCase.CreateTopic;
using TFA.Storage.Entities;

namespace TFA.Storage.Storages;

internal class CreateTopicStorage : ICreateTopicStorage
{
    private readonly IGuidFactory guidFactory;
    private readonly IMomentProvider momentProvider;
    private readonly ForumDbContext dbContext;
    private readonly IMapper mapper;

    public CreateTopicStorage(
        IGuidFactory guidFactory,
        IMomentProvider momentProvider,
        ForumDbContext dbContext,
        IMapper mapper
    )
    {
        this.guidFactory = guidFactory;
        this.momentProvider = momentProvider;
        this.dbContext = dbContext;
        this.mapper = mapper;
    }

    public async Task<Domain.Models.Topic> CreateTopic(Guid forumId, Guid userId, string title, CancellationToken cancellationToken)
    {
        Guid topicId = guidFactory.Create();

        Topic topic = new Topic
        {
            TopicId = topicId,
            ForumId = forumId,
            UserId = userId,
            Title = title,
            CreatedAt = momentProvider.Now
        };

        await dbContext.Topics.AddAsync(topic, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return await dbContext.Topics
            .Where(t => t.TopicId == topicId)
            .ProjectTo<Domain.Models.Topic>(mapper.ConfigurationProvider)
            .FirstAsync(cancellationToken);
    }
}