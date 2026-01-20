using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TFA.Domain.UseCase.SignIn;
using TFA.Storage.Entities;

namespace TFA.Storage.Storages;

internal class SignInStorage : ISignInStorage
{
    private readonly IGuidFactory guidFactory;
    private readonly ForumDbContext dbContext;
    private readonly IMapper mapper;

    public SignInStorage(
        IGuidFactory guidFactory,
        ForumDbContext dbContext,
        IMapper mapper
    )
    {
        this.guidFactory = guidFactory;
        this.dbContext = dbContext;
        this.mapper = mapper;
    }

    public Task<RecognizedUser?> FindUser(string login, CancellationToken cancellationToken) => dbContext.Users
        .Where(user => user.Login.Equals(login))
        .ProjectTo<RecognizedUser>(mapper.ConfigurationProvider)
        .FirstOrDefaultAsync(cancellationToken);

    public async Task<Guid> CreateSession(
        Guid userId, DateTimeOffset expirationMoment, CancellationToken cancellationToken)
    {
        var sessionId = guidFactory.Create();
        await dbContext.Sessions.AddAsync(new Session
        {
            SessionId = sessionId,
            UserId = userId,
            ExpiresAt = expirationMoment
        }, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return sessionId;
    }
}
