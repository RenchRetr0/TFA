namespace TFA.Domain.Exceptions;

public class ForumNotFoundException : DomainException
{
    public ForumNotFoundException(Guid forumId) : base(DomainErrorCode.Gone, $"Forum with id {forumId} was not found")
    { }
}

public abstract class DomainException : Exception
{
    public DomainErrorCode ErrorCode { get; }

    public DomainException(DomainErrorCode errorCode, string message) : base(message)
    {
        ErrorCode = errorCode;
    }
}