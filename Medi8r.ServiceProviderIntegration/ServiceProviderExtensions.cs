using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Medi8r.ServiceProviderIntegration;

public static class ServiceProviderExtensions
{
    private static List<Type> _notificationHandlerTypes = new();
    private static List<Type> _requestHandlerTypes = new();
    
    public static IServiceCollection RegisterMediator(this IServiceCollection serviceContainer,
        ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
    {
        var notificationHandlerTypes = Assembly.GetCallingAssembly().GetTypes()
            .Where(x => x.IsAssignableTo(typeof(INotificationHandler)))
            .ToList();

        var requestHandlerTypes = Assembly.GetCallingAssembly().GetTypes()
            .Where(x => x.IsAssignableTo(typeof(IRequestHandler)))
            .ToList();

        foreach (var handlerType in notificationHandlerTypes)
        {
            serviceContainer.Add(new ServiceDescriptor(handlerType, handlerType, serviceLifetime));
        }

        foreach (var handlerType in requestHandlerTypes)
        {
            serviceContainer.Add(new ServiceDescriptor(handlerType, handlerType, serviceLifetime));
        }

        _notificationHandlerTypes.AddRange(notificationHandlerTypes);
        _requestHandlerTypes.AddRange(requestHandlerTypes);
        
        serviceContainer.TryAdd(new ServiceDescriptor(typeof(IMediator), provider =>
        {
            return new Mediator(_notificationHandlerTypes, _requestHandlerTypes,
                type => provider.GetRequiredService(type));
        }, serviceLifetime));
        
        return serviceContainer;
    }
}