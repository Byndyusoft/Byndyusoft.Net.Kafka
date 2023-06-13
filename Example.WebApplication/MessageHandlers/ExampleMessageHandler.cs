using System.Threading.Tasks;
using Byndyusoft.Example.WebApplication.Dtos;
using KafkaFlow;
using KafkaFlow.TypedHandler;
using Microsoft.Extensions.Logging;

namespace Byndyusoft.Example.WebApplication.MessageHandlers;

public class ExampleMessageHandler : IMessageHandler<ExampleMessageDto>
{
    private readonly IExampleService _exampleService;

    public ExampleMessageHandler(IExampleService exampleService)
    {
        _exampleService = exampleService;
    }

    public async Task Handle(IMessageContext context, ExampleMessageDto message)
    {
        await _exampleService.DoSomething(message);
    }
}

public class ExampleService : IExampleService
{
    private readonly ILogger<ExampleService> _logger;

    public ExampleService(ILogger<ExampleService> logger)
    {
        _logger = logger;
    }

    public Task DoSomething(ExampleMessageDto message)
    {
        _logger.LogInformation("Arrived message with Guid: {Guid} and text {Text}", message.Guid, message.Text);
        
        //Do some work
        return Task.CompletedTask; 
    }
}

public interface IExampleService
{
    public Task DoSomething(ExampleMessageDto message);
}