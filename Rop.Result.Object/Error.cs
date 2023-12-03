namespace Rop.Result.Object;

public record Error()
{
    public required DomainError DomainError { get; init; }
    public string? Code { get; init; }
    public string? Message { get; init; }
}
