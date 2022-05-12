using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Medi8r;

public class Mediator : IMediator
{
    private readonly Func<Type, object> _handlerFactory;
    private Dictionary<Type, List<Type>> _notificationHandlers;
    private Dictionary<Type, Type> _voidRequestHandlers;
    private Dictionary<Type, Type> _valueRequestHandlers;

    public Mediator(IEnumerable<Type> notificationHandlers,
        IEnumerable<Type> requestHandlers,
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

        _voidRequestHandlers = requestHandlers.Select(x => new
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
        
        _valueRequestHandlers = requestHandlers.Select(x => new
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

    public async Task<TResponse> Send<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : IRequest<TResponse>
    {
        var requestType = typeof(TRequest);

        if (!_valueRequestHandlers.TryGetValue(requestType, out var handlerType)) 
            throw new RequestHandlerNotFoundException(requestType);

        var handler = (IRequestHandler) _handlerFactory.Invoke(handlerType);

        return (TResponse) await handler.Handle(request, cancellationToken);
    }
    
    public async Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : IRequest<Void>
    {
        var requestType = typeof(TRequest);

        if (!_voidRequestHandlers.TryGetValue(requestType, out var handlerType)) 
            throw new RequestHandlerNotFoundException(requestType);

        var handler = (IRequestHandler) _handlerFactory.Invoke(handlerType);

        await handler.Handle(request, cancellationToken);
    }
}