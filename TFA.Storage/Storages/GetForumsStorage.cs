using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using TFA.Domain.UseCase.GetForums;

namespace TFA.Storage.Storages;

internal class GetForumsStorage : IGetForumsStorage
{
    private readonly ForumDbContext dbContext;
    private readonly IMemoryCache memoryCache;
    private readonly IMapper mapper;

    public GetForumsStorage(
        IMemoryCache memoryCache,
        ForumDbContext dbContext,
        IMapper mapper)
    {
        this.memoryCache = memoryCache;
        this.dbContext = dbContext;
        this.mapper = mapper;
    }

    public async Task<IEnumerable<Domain.Models.Forum>> GetForums(CancellationToken cancellationToken)
    {
        var forums = await memoryCache.GetOrCreateAsync(
        nameof(GetForums), entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10);
            return dbContext.Forums
                .ProjectTo<Domain.Models.Forum>(mapper.ConfigurationProvider)
                .ToArrayAsync(cancellationToken);
        });

        return forums ?? Enumerable.Empty<Domain.Models.Forum>();
    }
}