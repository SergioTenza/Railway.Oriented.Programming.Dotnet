namespace Rop.Result.Object;

public sealed class Result<T>
{
    private readonly T? _content;
    public T Data => _content!;
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error[] Errors { get; }

    private Result(T value)
    {
        if (value is not null)
        {
            IsSuccess = true;
            Errors = [];
            _content = value;
        }
        else
        {
            IsSuccess = false;
            Errors = [new Error { DomainError = DomainError.NullValue }];
        }
    }
    private Result(Error error)
    {
        IsSuccess = false;
        Errors = [error];
    }
    private Result(Error[] errors)
    {
        IsSuccess = false;
        Errors = [.. errors];
    }

    public static Result<T> Success(T data) => new(data);
    public static Result<T> Failed(Error error) => new(error);
    public static Result<T> Failed(IEnumerable<Error> errors) => new(errors.ToArray());


    public static implicit operator Result<T>(T value) => new(value);

    public static implicit operator Result<T>(Error error) => new(error);

    public static implicit operator Result<T>(Error[] errors) => new(errors);
}
