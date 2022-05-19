using LabIOC;
using Medi8r.demo;
using Medi8r.LabIOCIntegration;

var iocContainer = LabContainerFactory.Create()
    .RegisterMediator()
    .Build();

var mediator = iocContainer.GetMediator();

await mediator.Publish(new DemoNotification());
var response = await mediator.Send(new DemoRequest());
Console.WriteLine(response);

await mediator.Send(new DemoVoidRequest());