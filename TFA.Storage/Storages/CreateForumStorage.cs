using Microsoft.EntityFrameworkCore;
using TFA.Domain.UseCase.CreateForum;

namespace TFA.Storage.Storages;

internal class CreateForumStorage : ICreateForumStorage
{
    private readonly IGuidFactory guidFactory;
    private readonly ForumDbContext dbContext;

    public CreateForumStorage(
        IGuidFactory guidFactory,
        ForumDbContext dbContext)
    {
        this.guidFactory = guidFactory;
        this.dbContext = dbContext;
    }
    public async Task<Domain.Models.Forum> Create(string title, CancellationToken cancellationToken)
    {
        var forumId = guidFactory.Create();
        var forum = new Forum
        {
            ForumId = forumId,
            Title = title,
        };
        await dbContext.Forums.AddAsync(forum, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return await dbContext.Forums
            .Where(f => f.ForumId == forumId)
            .Select(f => new Domain.Models.Forum
            {
                Id = f.ForumId,
                Title = f.Title,
            })
            .FirstAsync();
    }
}
