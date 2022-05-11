namespace Medi8r.demo;

public class DemoNotificationHandler2 : INotificationHandler<DemoNotification>
{
    public Task Handle(DemoNotification notification, CancellationToken cancellationToken = default)
    {
        Console.WriteLine("Notification handled like a b0ss B)");
        return Task.CompletedTask;
    }
}