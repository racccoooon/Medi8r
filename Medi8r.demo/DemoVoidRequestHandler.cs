namespace Medi8r.demo;

public class DemoVoidRequestHandler : IRequestHandler<DemoVoidRequest>
{
    public Task Handle(DemoVoidRequest request, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"No return type.");
        return Task.CompletedTask;
    }
}

public class DemoVoidRequest : IRequest<Void>
{
    
}