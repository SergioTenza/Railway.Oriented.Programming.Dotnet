using Rop.Result.Object;

namespace Rop.Masstransit.Cqrs;

public interface IRequestBus
{
    Task<Result<TResult>> GetResponse<TRequest, TResult>(TRequest message)
        where TRequest : class where TResult : class;
}
