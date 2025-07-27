namespace TFA.Domain.UseCase.GetForums;

internal class GetForumsUseCase : IGetForumsUseCase
{
    private readonly IGetForumsStorage storage;

    public GetForumsUseCase(IGetForumsStorage storage)
    {
        this.storage = storage;
    }
    public Task<IEnumerable<Models.Forum>> Execute(CancellationToken cancellationToken) =>
        storage.GetForums(cancellationToken);
}