using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Rop.Result.Object;

namespace Rop.Masstransit.Cqrs;

public class MassTransitRequestBus : IRequestBus
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public MassTransitRequestBus(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task <Result<TResult>> GetResponse<TRequest,TResult>(TRequest message)
        where TRequest : class
        where TResult : class
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var clientType = typeof(IRequestClient<>).MakeGenericType(typeof(TRequest));
        var client = scope.ServiceProvider.GetService(clientType);

        if(client is not IRequestClient<TRequest> requestClient)
        {
            throw new InvalidOperationException($"Could not create client for type {clientType}");
        }

        var response = await requestClient.GetResponse<TResult,Error[]>(message);

        if(response.Is(out Response<Error[]>? validationResult) && validationResult is not null)
        {
            return validationResult.Message;
        }

        if(response.Message is TResult requestResult)
        {
            return requestResult;
        }
        throw new InvalidOperationException($"Response is not of the expected type {typeof(TResult)}");
    }
}
