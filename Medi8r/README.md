# Medi8r

Easy to use implementation of the mediator design pattern.

## Usage/Examples

The following examples show some basic usages of this package:

### Notification Handler
```csharp
public class DemoNotification : INotification {}

public class DemoNotificationHandler : INotificationHandler<DemoNotification>
{
    public Task Handle(DemoNotification notification, CancellationToken cancellationToken = default)
    {
        Console.WriteLine("Demo notification handled successfully.")
        return Task.CompletedTask;
    }
}
```

### Notification Behaviour
```csharp
public class DemoNotificationBehaviour : INotificationBehaviour<DemoNotification>
{
    public async Task Handle(DemoNotification notification, Func<DemoNotification, CancellationToken, Task> next, CancellationToken cancellationToken = default)
    {
        Console.WriteLine("Started Notification Handling");

        await next(notification, cancellationToken);
        
        Console.WriteLine("Finished Notification Handling");
    }
}
```

### Request Handler
```csharp
public class DemoRequest : IRequest<DemoResponse> {}
public class DemoResponse {}

public class DemoRequestHandler : IRequestHandler<DemoRequest, DemoResponse>
{
    public Task<DemoResponse> Handle(DemoRequest request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new DemoResponse());
    }
}
```

### Mediator Object
```csharp
var mediator = new Mediator(...);

// publish new notification
await mediator.Publish(new DemoNotification());

// send a request
var response = await mediator.Send(new DemoRequest());
```


## License

[ MIT ](https://choosealicense.com/licenses/mit/)