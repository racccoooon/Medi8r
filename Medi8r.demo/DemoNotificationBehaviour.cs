namespace Medi8r.demo;

public class DemoNotificationBehaviour : INotificationBehaviour<DemoNotification>
{
    public async Task Handle(DemoNotification notification, Func<DemoNotification, CancellationToken, Task> next, CancellationToken cancellationToken = default)
    {
        Console.WriteLine("Started Notification Handling");

        await next(notification, cancellationToken);
        
        Console.WriteLine("Finished Notification Handling");
    }
}

public class DemoNotificationBehaviour2 : INotificationBehaviour<DemoNotification>
{
    public async Task Handle(DemoNotification notification, Func<DemoNotification, CancellationToken, Task> next, CancellationToken cancellationToken = default)
    {
        Console.WriteLine("Inner handler");

        await next(notification, cancellationToken);
        
        Console.WriteLine("Finished inner handler");
    }
}