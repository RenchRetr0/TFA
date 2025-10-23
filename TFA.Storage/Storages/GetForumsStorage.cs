using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using TFA.Domain.UseCase.GetForums;

namespace TFA.Storage.Storages;

internal class GetForumsStorage : IGetForumsStorage
{
    private readonly ForumDbContext dbContext;
    private readonly IMemoryCache memoryCache;

    public GetForumsStorage(
        IMemoryCache memoryCache,
        ForumDbContext dbContext)
    {
        this.memoryCache = memoryCache;
        this.dbContext = dbContext;
    }

    public async Task<IEnumerable<Domain.Models.Forum>> GetForums(CancellationToken cancellationToken)
    {
        var forums = await memoryCache.GetOrCreateAsync(
        nameof(GetForums), entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10);
            return dbContext.Forums
                .Select(f => new Domain.Models.Forum
                {
                    Id = f.ForumId,
                    Title = f.Title
                })
                .ToArrayAsync(cancellationToken);
        });

        return forums ?? Enumerable.Empty<Domain.Models.Forum>();
    }
}