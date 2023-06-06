using KafkaFlow.TypedHandler;

namespace Byndyusoft.Net.Kafka
{
    public interface IKafkaConsumer
    {
        public string Topic { get; }
        public IMessageHandler MessageHandler { get; }
    }
}