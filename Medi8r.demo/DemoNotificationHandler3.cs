namespace Medi8r.demo;

public class DemoNotificationHandler3 : INotificationHandler<DemoNotification>
{
    public Task Handle(DemoNotification notification, CancellationToken cancellationToken = default)
    {
        Console.WriteLine("deez nuts");
        return Task.CompletedTask;
    }
}