namespace Medi8r;

public interface IRequestHandler<TRequest, TResponse> : IRequestHandler
    where TRequest : IRequest<TResponse>
{
    Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken = default);

    async Task<object> IRequestHandler.Handle(object request, CancellationToken cancellationToken)
    {
        return (await Handle((TRequest) request, cancellationToken))!;
    }
}

public interface IRequestHandler<TRequest> : IRequestHandler<TRequest, Void>
    where TRequest : IRequest<Void>
{
    new Task Handle(TRequest request, CancellationToken cancellationToken = default);
    
    async Task<Void> IRequestHandler<TRequest, Void>.Handle(TRequest request, CancellationToken cancellationToken)
    {
        await Handle(request, cancellationToken);
        return Void.Value;
    }
}

public interface IRequestHandler
{
    Task<object> Handle(object request, CancellationToken cancellationToken = default);
}