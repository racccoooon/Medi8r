namespace Medi8r;

public class RequestHandlerNotFoundException : Exception
{
    public RequestHandlerNotFoundException(Type requestType)
        : base($"No request handler found for type {requestType.FullName}")
    {
    }
}