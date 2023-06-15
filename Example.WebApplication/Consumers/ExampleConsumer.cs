using Byndyusoft.Example.Domain.MessageHandlers;
using Byndyusoft.Net.Kafka.Abstractions;
using KafkaFlow.TypedHandler;

namespace Byndyusoft.Example.WebApplication.Consumers
{
    public sealed class ExampleConsumer : IKafkaConsumer
    {
        public ExampleConsumer(ExampleMessageHandler messageHandler)
        {
            MessageHandler = messageHandler;
        }   
        public string Topic => "topic";

        public string GroupName => "example_group";
        public IMessageHandler MessageHandler { get; }
    }
}