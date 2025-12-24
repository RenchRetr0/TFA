using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TFA.Domain.UseCase.SignIn;

namespace TFA.Storage.Storages;

internal class SignInStorage : ISignInStorage
{
    private readonly ForumDbContext dbContext;
    private readonly IMapper mapper;

    public SignInStorage(
        ForumDbContext dbContext,
        IMapper mapper
    )
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
    }

    public Task<RecognizedUser?> FindUser(string login, CancellationToken cancellationToken) => dbContext.Users
        .Where(user => user.Login.Equals(login))
        .ProjectTo<RecognizedUser>(mapper.ConfigurationProvider)
        .FirstOrDefaultAsync(cancellationToken);
}
