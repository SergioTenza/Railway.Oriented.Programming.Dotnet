# Railway Oriente Programming with dotnet - chsarp - Masstransit

## Result implementation

```csharp
public class Result<T>
{
    private T? _content;
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error[] Errors { get; }

    private Result(T value)
    {
        if(value is null)        
        {
            IsSuccess = true;
            Errors = [];
            _content = value;    
        }
        else
        {
            IsSuccess = false;
            Errors = [Error.NullValue];
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
}
```

### Result -> Match -> Only way to access value

```csharp
public TResult Match<TResult>(
    Func<T,TResult> success,
    Func<Error[],TResult> failure) =>
    !IsFailure ? success(_content!) : failure(Errors!);
public TResult Match<TResult>(
    Func<T,TResult> success,
    Func<Error,TResult> failure) =>
    !IsFailure ? success(_content!) : failure(Errors.First()!);
```

### Result -> Combine -> Mixes multiple Results onto one Result

```csharp
public static Result<TValue> Combine<TValue>(params Result<TValue>[] results)
{
    if(results.Any(r => r.IsFailure))
    {
        return 
            results
                .SelectMany(r=> r.Errors)
                .Where(e=> e != Error.None)
                .Distinct()
                .ToArray();
    }
    return results[0];
}
```

### Error Enum

```csharp
public enum Error
{
    None,
    NullValue
}
```

## Masstransit CQRS Implementation

Program.cs

```csharp
builder.Services.AddTransient<IRequestBus, MassTransitRequestBus>();
builder.Services.AddMassTransit(busConfigurator =>
{
    busConfigurator.UsingInMemory();
    busConfigurator.AddMediator(x =>
    {
        x.AddConsumers(typeof(Program).Assembly);
        x.AddRequestClient(typeof(GetOrderStatus));
    });
});
```

Controller

```csharp
[Route("[controller]")]
public class OrdersController : Controller
{
    private readonly IRequestBus requestBus;

    public OrdersController(IRequestBus requestBus)
    {
        this.requestBus = requestBus;
    }

    [HttpGet]
    public async Task<IActionResult> Get(int orderId)
    {
        var response = await requestBus.GetResponse<GetOrderStatus, OrderStatusResult>(new GetOrderStatus(orderId));

        if (response.IsValid)
        {
            return new OkObjectResult(response.Result);
        }

        return new BadRequestObjectResult(response.ValidationErrors);
    }
}
```

Request Handler

```csharp
public class OrderStatusRequestHandler : RequestHandler<GetOrderStatus, OrderStatusResult>
{
    private readonly OrderProvider provider;

    public OrderStatusRequestHandler(OrderProvider provider)
    {
        this.provider = provider;
    }

    protected override async Task<ValidationErrors> Validate(GetOrderStatus request, CancellationToken token)
    {
        if (request.OrderId <= 0)
        {
            return new ValidationErrors(new[] {$"Order ID {request.OrderId} is invalid."});
        }

        return new ValidationErrors();
    }

    protected override async Task<OrderStatusResult> Handle(GetOrderStatus request, CancellationToken cancellationToken)
    {
        var order = await provider.GetByIdAsync(request.OrderId);

        return new OrderStatusResult(order.Id, order.State);
    }
}
```
