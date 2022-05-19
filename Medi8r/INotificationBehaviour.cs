namespace Medi8r;

public interface INotificationBehaviour<TNotification> : INotificationBehaviour
    where TNotification : INotification
{
    Task INotificationBehaviour.Handle(INotification notification, Func<INotification, CancellationToken, Task> next, CancellationToken cancellationToken)
    {
        return Handle((TNotification)notification, (n, c) => next(n, c), cancellationToken);
    }
    
    Task Handle(TNotification notification, Func<TNotification, CancellationToken, Task> next, CancellationToken cancellationToken = default);
}

public interface INotificationBehaviour
{
    Task Handle(INotification notification, Func<INotification, CancellationToken, Task> next, CancellationToken cancellationToken = default);
}