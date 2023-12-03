# Masstransit CQRS Implementation

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
