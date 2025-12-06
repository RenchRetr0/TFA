using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TFA.Domain.UseCase.GetTopics;

namespace TFA.Storage.Storages;

internal class GetTopicsStorage : IGetTopicsStorage
{
    private readonly ForumDbContext dbContext;
    private readonly IMapper mapper;

    public GetTopicsStorage(
        ForumDbContext dbContext,
        IMapper mapper
    )
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
    }

    public async Task<(IEnumerable<Domain.Models.Topic> resources, int totalCount)> GetTopics(
        Guid forumId, int skip, int take, CancellationToken cancellationToken)
    {
        var query = dbContext.Topics.Where(t => t.ForumId == forumId);

        var totalCount = await query.CountAsync(cancellationToken);

        var resources = await dbContext.Topics
            .Where(t => t.ForumId == forumId)
            .ProjectTo<Domain.Models.Topic>(mapper.ConfigurationProvider)
            .Skip(skip)
            .Take(take)
            .ToArrayAsync(cancellationToken);

        return (resources, totalCount);
    }
}
