namespace Medi8r.demo;

public class DemoRequestHandler : IRequestHandler<DemoRequest, DemoResponse>
{
    public Task<DemoResponse> Handle(DemoRequest request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new DemoResponse());
    }
}