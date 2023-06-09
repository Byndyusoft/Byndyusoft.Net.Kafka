using System.Threading.Tasks;
using Byndyusoft.Example.WebApplication.Dtos;
using KafkaFlow;
using KafkaFlow.TypedHandler;
using Microsoft.Extensions.Logging;

namespace Byndyusoft.Example.WebApplication.MessageHandlers;

public class ExampleMessageHandler : IMessageHandler<ExampleMessageDto>
{
    private readonly ILogger<ExampleMessageHandler> _logger;

    public ExampleMessageHandler(ILogger<ExampleMessageHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(IMessageContext context, ExampleMessageDto message)
    {
        _logger.LogInformation("Arrived message with Id: {Id} and text {Text}", message.Id, message.Text);
            
        // Полезная работа
        await Task.Delay(1);
    }
}