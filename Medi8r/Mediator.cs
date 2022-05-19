using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Medi8r;

public class Mediator : IMediator
{
    private readonly Func<Type, object> _handlerFactory;
    private Dictionary<Type, List<Type>> _notificationHandlers;
    private Dictionary<Type, List<Type>> _notificationBehaviours;
    private Dictionary<Type, Type> _voidRequestHandlers;
    private Dictionary<Type, Type> _valueRequestHandlers;

    public Mediator(IEnumerable<Type> notificationHandlers,
        IEnumerable<Type> requestHandlers,
        IEnumerable<Type> notificationBehaviours,
        IEnumerable<Type> requestBehaviours,
        Func<Type, object> handlerFactory)
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

        _notificationBehaviours = notificationBehaviours.Select(x => new
            {
                BehaviourType = x,
                NotificationType = x.GetInterfaces().FirstOrDefault(@interface =>
                    @interface.IsGenericType &&
                    @interface.GetGenericTypeDefinition() == typeof(INotificationBehaviour<>))
            })
            .Where(x => x.NotificationType != null)
            .GroupBy(x => x.NotificationType)
            .Select(x => new
            {
                NotificationType = x.Key!.GenericTypeArguments[0],
                BehaviourTypes = x.Select(group => group.BehaviourType).ToList()
            })
            .ToDictionary(x => x.NotificationType, x => x.BehaviourTypes);

        var requestHandlerList = requestHandlers.ToList();
        _voidRequestHandlers = requestHandlerList.Select(x => new
            {
                HandlerType = x,
                RequestType = x.GetInterfaces().FirstOrDefault(@interface =>
                    @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IRequestHandler<>))
            })
            .Where(x => x.RequestType != null)
            .Select(x => new
            {
                RequestType = x.RequestType!.GenericTypeArguments[0], x.HandlerType
            })
            .ToDictionary(x => x.RequestType!, x => x.HandlerType);

        _valueRequestHandlers = requestHandlerList.Select(x => new
            {
                HandlerType = x,
                RequestType = x.GetInterfaces().FirstOrDefault(@interface =>
                    @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IRequestHandler<,>))
            })
            .Where(x => x.RequestType != null)
            .Select(x => new
            {
                RequestType = x.RequestType!.GenericTypeArguments[0], x.HandlerType
            })
            .ToDictionary(x => x.RequestType!, x => x.HandlerType);
    }

    public async Task Publish(INotification notification, CancellationToken cancellationToken = default)
    {
        var notificationType = notification.GetType();

        if (!_notificationHandlers.TryGetValue(notificationType, out var handlerTypes))
        {
            return;
        }

        var handlers = handlerTypes
            .Select(x => (INotificationHandler)_handlerFactory.Invoke(x))
            .ToList();

        foreach (var handler in handlers)
        {
            var callTree = _notificationBehaviours
                .GetValueOrDefault(notificationType, new List<Type>())
                .Select(x => (INotificationBehaviour)_handlerFactory.Invoke(x))
                .Reverse()
                .Aggregate((Func<INotification, CancellationToken, Task>)handler.Handle, (a, b) =>
                    async (n, c) => await b.Handle(n, a, c));

            await callTree.Invoke(notification, cancellationToken);
        }
    }

    private class WrappedNotificationHandler : INotificationBehaviour
    {
        private readonly INotificationHandler _handler;

        public WrappedNotificationHandler(INotificationHandler handler)
        {
            _handler = handler;
        }

        public async Task Handle(INotification notification, Func<INotification, CancellationToken, Task> next,
            CancellationToken cancellationToken = default)
        {
            await _handler.Handle(notification, cancellationToken);
        }
    }

    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request,
        CancellationToken cancellationToken = default)
    {
        var requestType = request.GetType();

        if (!_valueRequestHandlers.TryGetValue(requestType, out var handlerType))
            throw new RequestHandlerNotFoundException(requestType);

        var handler = (IRequestHandler)_handlerFactory.Invoke(handlerType);

        return (TResponse)await handler.Handle(request, cancellationToken);
    }

    public async Task Send(IRequest<Void> request, CancellationToken cancellationToken = default)
    {
        var requestType = request.GetType();

        if (!_voidRequestHandlers.TryGetValue(requestType, out var handlerType))
            throw new RequestHandlerNotFoundException(requestType);

        var handler = (IRequestHandler)_handlerFactory.Invoke(handlerType);

        await handler.Handle(request, cancellationToken);
    }
}