namespace Medi8r;

public interface INotificationHandler
{
    Task Handle(INotification notification, CancellationToken cancellationToken = default);
}

public interface INotificationHandler<TNotification> : INotificationHandler
    where TNotification : INotification
{
    Task INotificationHandler.Handle(INotification notification, CancellationToken cancellationToken)
    {
        return Handle((TNotification) notification, cancellationToken);
    }
    
    Task Handle(TNotification notification, CancellationToken cancellationToken = default);
}