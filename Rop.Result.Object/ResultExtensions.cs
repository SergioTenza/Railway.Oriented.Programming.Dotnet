namespace Rop.Result.Object.Extensions;

public static class ResultExtensions
{
    public static Result<TOutData> OnSuccess<TInData, TOutData>(
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
}
