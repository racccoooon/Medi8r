using System.Reflection;
using LabIOC;

namespace Medi8r.LabIOCIntegration;

public static class LabIOCExtensions
{
    public static LabContainerFactory RegisterMediator(this LabContainerFactory containerFactory)
    {
        var notificationHandlerTypes = Assembly.GetCallingAssembly().GetTypes()
            .Where(x => x.IsAssignableTo(typeof(INotificationHandler)))
            .ToList();

        foreach (var handlerType in notificationHandlerTypes)
        {
            containerFactory.Register(handlerType);
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
        var mediator = new Mediator(notificationHandlerTypes, type => container.Get(type));
        return mediator;
    }
}