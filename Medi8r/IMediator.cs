namespace Medi8r;

public interface IMediator
{
    Task Publish(INotification notification, CancellationToken cancellationToken = default);
    Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);
    Task Send(IRequest<Void> request, CancellationToken cancellationToken = default);
}