using Microsoft.EntityFrameworkCore;
using TFA.Domain.Models;
using TFA.Storage;

namespace TFA.Domain.UseCase.GetForums;

public class GetForumsUseCase : IGetForumsUseCase
{
    private readonly ForumDbContext forumDbContext;

    public GetForumsUseCase(ForumDbContext forumDbContext)
    {
        this.forumDbContext = forumDbContext;
    }
    public async Task<IEnumerable<Models.Forum>> Execute(CancellationToken cancellationToken) =>
        await forumDbContext.Forums
            .Select(f => new Models.Forum
            {
                Id = f.ForumId,
                Title = f.Title
            })
            .ToArrayAsync(cancellationToken);
}