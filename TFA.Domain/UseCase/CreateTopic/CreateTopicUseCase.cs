using Microsoft.EntityFrameworkCore;
using TFA.Domain.Exceptions;
using TFA.Domain.Models;
using TFA.Storage;

namespace TFA.Domain.UseCase.CreateTopic;

public class CreateTopicUseCase : ICreateTopicUseCase
{
    private readonly ForumDbContext dbContext;
    private readonly IMomentProvider momentProvider;
    private readonly IGuidFactory guidFactory;

    public CreateTopicUseCase(
        IGuidFactory guidFactory,
        IMomentProvider momentProvider,
        ForumDbContext dbContext
    )
    {
        this.dbContext = dbContext;
        this.guidFactory = guidFactory;
        this.momentProvider = momentProvider;
    }

    public async Task<Models.Topic> Execute(Guid forumId, string Title, Guid authorId, CancellationToken cancellationToken)
    {
        var forumExist = await dbContext.Forums.AnyAsync(f => f.ForumId == forumId, cancellationToken);
        if(!forumExist)
        {
            throw new ForumNotFoundException(forumId);
        }

        Guid topicId = guidFactory.Create();
        
        await dbContext.Topics.AddAsync(new Storage.Topic{
            TopicId = topicId,
            ForumId = forumId,
            UserId = authorId,
            CreatedAt = momentProvider.Now,
            Title = Title,
        }, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return await dbContext.Topics
            .Where(t => t.TopicId == topicId)
            .Select(t => new Models.Topic
            {
                Id = t.TopicId,
                Title = t.Title,
                CreatedAt = t.CreatedAt,
                Author = t.Author!.Login
            })
            .FirstAsync(cancellationToken);
    }
}