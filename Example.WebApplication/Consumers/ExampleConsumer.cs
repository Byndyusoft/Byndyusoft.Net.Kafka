using System.Threading.Tasks;
using Byndyusoft.Example.WebApplication.Producers;
using Byndyusoft.Net.Kafka;
using KafkaFlow;
using KafkaFlow.TypedHandler;
using Microsoft.Extensions.Logging;

namespace Byndyusoft.Example.WebApplication.Consumers
{
    public class ExampleConsumer : IKafkaConsumer
    {
        public ExampleConsumer(ExampleMessageHandler exampleMessageHandler)
        {
            MessageHandler = exampleMessageHandler;
        }
        public string Topic => "topic";
        public IMessageHandler MessageHandler { get; }
    }

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
            await Task.Delay(100);
        }
    }
}