using Byndyusoft.Example.WebApplication.Dtos;
using Byndyusoft.Net.Kafka;
using KafkaFlow.TypedHandler;

namespace Byndyusoft.Example.WebApplication.Consumers
{
    public class ExampleConsumer : IKafkaConsumer
    {
        public ExampleConsumer(IMessageHandler<ExampleMessageDto> messageHandler)
        {
            MessageHandler = messageHandler;
        }   
        public string Topic => "topic";
        public IMessageHandler MessageHandler { get; }
    }
}