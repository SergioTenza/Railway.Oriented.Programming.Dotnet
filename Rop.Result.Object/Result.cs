﻿namespace Rop.Result.Object;

public class Result<T>
{
    private T? _content;
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

    public static implicit operator Result<T>(T value) => new(value);

    public static implicit operator Result<T>(Error error) => new(error);

    public static implicit operator Result<T>(Error[] errors) => new(errors);

    public Result<TResult> Match<TResult>(
        Func<T, TResult> success,
        Func<T, TResult> failure) =>
        !IsFailure ? success(_content!) : failure(Errors!);
    public static Result<TValue> Combine<TValue>(params Result<TValue>[] results)
    {
        if (results.Any(r => r.IsFailure))
        {
            return
                results
                    .SelectMany(r => r.Errors)
                    .Where(e => e.DomainError != DomainError.None)
                    .Distinct()
                    .ToArray();
        }
        return results[0];
    }
}
