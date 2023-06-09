using Byndyusoft.Example.WebApplication.MessageHandlers;
using Byndyusoft.Net.Kafka;
using KafkaFlow.TypedHandler;

namespace Byndyusoft.Example.WebApplication.Consumers
{
    public class ExampleConsumer : IKafkaConsumer
    {
        public ExampleConsumer(ExampleMessageHandler messageHandler)
        {
            MessageHandler = messageHandler;
        }   
        public string Topic => "topic";
        public IMessageHandler MessageHandler { get; }
    }
}