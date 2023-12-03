using MassTransit;
using MassTransit.Mediator;
using Rop.Result.Object;

namespace Rop.Masstransit.Cqrs;
public abstract class RequestHandler<TRequest, TResponse> : IConsumer<TRequest>
    where TRequest : class, Request<TResponse>
    where TResponse : class
{
    public async Task Consume(ConsumeContext<TRequest> context)
    {
        var validationResult = await Validate(context.Message, context.CancellationToken);
        if (validationResult.Any())
        {
            await context.RespondAsync(validationResult);
            return;
        }
        
        var response = await Handle(context.Message, context.CancellationToken).ConfigureAwait(false);
        await context.RespondAsync(response);
    }

    protected virtual Task<Error[]> Validate(TRequest request, CancellationToken token)
    {
        return Task.FromResult(Array.Empty<Error>());
    }

    protected abstract Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}