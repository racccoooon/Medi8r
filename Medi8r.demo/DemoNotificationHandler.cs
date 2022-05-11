namespace Medi8r.demo;

public class DemoNotificationHandler : INotificationHandler<DemoNotification>
{
    public Task Handle(DemoNotification notification, CancellationToken cancellationToken = default)
    {
        Console.WriteLine("Notification handled");
        return Task.CompletedTask;
    }
}