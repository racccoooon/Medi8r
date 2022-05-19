using System.Reflection;
using LabIOC;

namespace Medi8r.LabIOCIntegration;

public static class LabIocExtensions
{
    public static LabContainerFactory RegisterMediator(this LabContainerFactory containerFactory)
    {
        var notificationHandlerTypes = Assembly.GetCallingAssembly().GetTypes()
            .Where(x => x.IsAssignableTo(typeof(INotificationHandler)))
            .ToList();

        var requestHandlerTypes = Assembly.GetCallingAssembly().GetTypes()
            .Where(x => x.IsAssignableTo(typeof(IRequestHandler)))
            .ToList();

        var notificationBehaviourTypes = Assembly.GetCallingAssembly().GetTypes()
            .Where(x => x.IsAssignableTo(typeof(INotificationBehaviour)))
            .ToList();

        foreach (var handlerType in notificationHandlerTypes)
        {
            containerFactory.Register(handlerType);
        }

        foreach (var handlerType in requestHandlerTypes)
        {
            containerFactory.Register(handlerType);
        }

        foreach (var notificationBehaviourType in notificationBehaviourTypes)
        {
            containerFactory.Register(notificationBehaviourType);
        }

        return containerFactory;
    }

    public static IMediator GetMediator(this LabContainer container)
    {
        var notificationHandlerTypes = container.GetMappings()
            .Where(x => x.ImplementationType == x.InterfaceType)
            .Where(x => x.ImplementationType.IsAssignableTo(typeof(INotificationHandler)))
            .Select(x => x.ImplementationType)
            .ToList();

        var requestHandlerTypes = container.GetMappings()
            .Where(x => x.ImplementationType == x.InterfaceType)
            .Where(x => x.ImplementationType.IsAssignableTo(typeof(IRequestHandler)))
            .Select(x => x.ImplementationType)
            .ToList();

        var notificationBehaviourTypes = container.GetMappings()
            .Where(x => x.ImplementationType == x.InterfaceType)
            .Where(x => x.ImplementationType.IsAssignableTo(typeof(INotificationBehaviour)))
            .Select(x => x.ImplementationType)
            .ToList();
        
        var mediator = new Mediator(notificationHandlerTypes, 
            requestHandlerTypes, 
            notificationBehaviourTypes,
            new List<Type>(),
            type => container.Get(type));
        return mediator;
    }
}