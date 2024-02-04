using Microsoft.AspNetCore.Http;

namespace Rop.Result.Object.Extensions;

public static class ResultExtensions
{
    public static Result<TOutData> Bind<TInData, TOutData>(
    this Result<TInData> input,
    Func<TInData, Result<TOutData>> switchFunction) =>
        input.IsSuccess
            ? switchFunction(input.Data)
            : input.Errors;
    public static Result<TOutData> Map<TInData, TOutData>(
    this Result<TInData> input,
    Func<TInData, TOutData> singleTrackFunction) =>
        input.IsSuccess
            ? singleTrackFunction(input.Data)
            : input.Errors;
    public static Result<TOutData> DoubleMap<TInData, TOutData>(
    this Result<TInData> input,
    Func<TInData, TOutData> successSingleTrackFunction,
    Func<TInData, TOutData> failureSingleTrackFunction)
    {
        if (input.IsSuccess)
        {
            return successSingleTrackFunction(input.Data);
        }
        failureSingleTrackFunction(input.Data);
        return input.Errors;
    }
    public static Result<TData> Tee<TData>(
        this Result<TData> input,
        Action<TData> deadEndFunction)
    {
        if (input.IsSuccess)
            deadEndFunction(input.Data);
        return input;
    }
    public static Result<TOutData> Succeed<TInData, TOutData>(
        this Result<TInData> input,
        Func<TInData, TOutData> singleTrackFunction) =>
            singleTrackFunction(input.Data);
    public static Result<TOutData> Fail<TInData, TOutData>(
        this Result<TInData> input,
        Func<TInData, TOutData> singleTrackFunction)
    {
        singleTrackFunction(input.Data);
        return new List<Error>
                {
                    new Error
                    {
                        DomainError = DomainError.Fail
                    }
                }.ToArray();
    }

    public static Result<TData> Apply<TData,TInput>(
        this Result<TData> input,
        Result<TInput> input2,
        Func<Result<TData>,Result<TInput>,Result<TData>> applyFunction)
    {
        if (input.IsSuccess)
        {
            return applyFunction(input.Data,input2.Data);
        }
        return input.Errors;
    }
     public static Result<TData> Apply<TData,TInput>(
        this Result<TData> input,
        TInput input2,
        Func<Result<TData>,TInput,Result<TData>> applyFunction)
    {
        if (input.IsSuccess)
        {
            return applyFunction(input.Data,input2);
        }
        return input.Errors;
    }
    public static IResult Handle<TData>(this Result<TData> result)
    {
        if(result.IsSuccess)
            return TypedResults.Ok(result.Data);

        var message = string.Join(';',result.Errors.Select(e => e.Message));
        return TypedResults.Problem(message);
    }
    public static Result<TOutData> TryCatchSwitch<TInData,TOutData>(this Result<TInData> input,Func<TInData,TOutData> singleTrackFunction)
    {
        try
        {
            return input.IsSuccess            
            ? Result<TOutData>.Success(singleTrackFunction(input.Data))
            : Result<TOutData>.Failed(input.Errors);
        }
        catch(Exception ex)
        {
            return Result<TOutData>.Failed(new Error
            {
                Message = ex.Message,
                Code = "3",
                DomainError = DomainError.UnhandledException

            });
        };
    }
    public static Result<bool> BooleanSwitch<TInData>(this Result<TInData> input,Func<TInData,bool> singleTrackFunction)
    {                   
        if(input.IsSuccess)
            return singleTrackFunction(input.Data)
                ? Result<bool>.Success(true)
                : Result<bool>.Failed(new Error
                {
                    DomainError = DomainError.BooleanSwitchFailed
                });
        return Result<bool>.Failed(input.Errors);
    }
}
