# Railway Oriented Programming with dotnet - chsarp - Masstransit

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

### Single Track Function

- A function that returns a Value  
  - INPUT
    =============== OUTPUT

```csharp
public bool IsAmountValid() => amount >= 0;
```

### Two Track Function

- A function that returns a Result Type
  - INPUT
    ===============>     SUCCESS
    ===============>     FAILURE

```csharp
public Result<decimal> ValidateAmount(decimal amount) =>
    amount >= 0 
        ? amount
        : [new Error { DomainError = DomainError.AccountValidationFailed }]
```

### Dead end Function

- A function wich doesn´t returns anything
  - INPUT
    ===============||

```csharp
AccountService.UpdateCard(request.CardCode, request.Amount);
```

### Supervisory

- Supervise both tracks (Logging,Metrics)
  - SUCCESS =============== SUCCESS
  - FAILURE =============== FAILURE

```csharp
Logger.Error($"Card topped up for {request.CardHolder}");
Logger.Info($"Card topped up for {request.CardHolder}");
```

### Switches

- Constructors
  - Create a new track
- Adapters
  - Convert one kind of track into another
- Combiners
  - Joining many tracks

### Bind

- Adapter block
- Two track in, Two track out
- Only executes on the success track
  - SUCCESS =======//====== SUCCESS
  - FAILURE ======//======= FAILURE

```csharp
public static class ResultExtensions
{
    public static Result<TOutData> Bind<TInData, TOutData>(
    Result<TInData> input,
    Func<Result<TInData>, Result<TOutData>> switchFunction) =>
        input.IsSuccess 
            ? switchFunction(input)
            : input.Errors;

}
```

### Map

- Adapter block
- One track in, Two track out
- Only executes on the success track
  - SUCCESS =============== SUCCESS
  - FAILURE =============== FAILURE

```csharp
public static Result<TOutData> Map<TInData, TOutData>(
    this.Result<TInData> input,
    Func<TInData, TOutData> singleTrackFunction) =>
        input.IsSuccess 
            ? singleTrackFunction(input.Data)
            : Result<TOutData>.Fail(input.Error);
```

### Double Map

- Supervisory functions
- Executes on both tracks
  - SUCCESS =============== SUCCESS
  - FAILURE =============== FAILURE

```csharp
public static Result<TOutData> DoubleMap<TInData, TOutData>(
    this.Result<TInData> input,
    Func<TInData, TOutData> successSingleTrackFunction,
    Func<TInData, TOutData> failureSingleTrackFunction)
    {
        if(input.IsSuccess)
        {
            return successSingleTrackFunction
        }
        input.IsSuccess 
            ? singleTrackFunction(input.Data)
            : Result<TOutData>.Fail(input.Error);
    }
        
```

### Tee

- Adapter block
- Takes a dead end function
- Turns it into a one track
  - SUCCESS =============== SUCCESS
  - ==============================|

```csharp
public static Result<TData> Tee<TData>(
    this Result<TData> input, 
    Action<TData> deadEndFunction)
    {
    if(input.IsSuccess)
        deadEndFunction(input.Data);
    return input;
    }        
```

### Succeed

- Constructor
- Always on the success track
- One track into Two tracks
  - INPUT   =============== SUCCESS
  - =============//======== FAILURE

```csharp
public static Result<TOutData> Succeed<TInData, TOutData>(
    this Result<TInData> input,
    Func<TInData, TOutData> singleTrackFunction) => 
        singleTrackFunction(input.Data);
```

### Fail

- Constructor
- Always on the failure track
- One track into Two tracks
  - INPUT   =============== SUCCESS
  - =============//======== FAILURE

```csharp
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
```

<https://www.youtube.com/watch?v=45yk2nuRjj8> (24:00);

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
