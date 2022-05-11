using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Medi8r;

public class Mediator : IMediator
{
    private readonly Func<Type, object> _handlerFactory;
    private Dictionary<Type, List<Type>> _notificationHandlers;

    public Mediator(IEnumerable<Type> notificationHandlers, Func<Type, object> handlerFactory)
    {
        _handlerFactory = handlerFactory;
        _notificationHandlers = notificationHandlers.Select(x => new
            {
                HandlerType = x,
                NotificationType = x.GetInterfaces().FirstOrDefault(@interface =>
                    @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(INotificationHandler<>))
            })
            .Where(x => x.NotificationType != null)
            .GroupBy(x => x.NotificationType)
            .Select(x => new
            {
                NotificationType = x.Key!.GenericTypeArguments[0],
                HandlerTypes = x.Select(group => group.HandlerType).ToList()
            })
            .ToDictionary(x => x.NotificationType!, x => x.HandlerTypes);
    }

    public async Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification
    {
        var notificationType = typeof(TNotification);
        
        if (!_notificationHandlers.TryGetValue(notificationType, out var handlerTypes))
        {
            return;
        }

        var handlers = handlerTypes
            .Select(x => (INotificationHandler) _handlerFactory.Invoke(x))
            .ToList();
        
        foreach (var handler in handlers)
        {
            await handler.Handle(notification, cancellationToken);
        }
    }

    public Task<TResponse> Send<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : IRequest<TResponse>
    {
        throw new NotImplementedException();
    }
}