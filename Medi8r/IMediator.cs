namespace Medi8r;

public interface IMediator
{
    Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification;
    Task<TResponse> Send<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default) 
        where TRequest : IRequest<TResponse>;
    Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default) 
        where TRequest : IRequest<Void>;
}