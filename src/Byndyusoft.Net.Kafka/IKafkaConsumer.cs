using KafkaFlow.TypedHandler;

namespace Byndyusoft.Net.Kafka
{
    /// <summary>
    ///     Consumer that handle messages from kafka
    /// </summary>
    public interface IKafkaConsumer
    {
        /// <summary>
        ///     Kafka topic name
        /// </summary>
        public string Topic { get; }
        
        /// <summary>
        ///     Message handling method
        /// </summary>
        public IMessageHandler MessageHandler { get; }
    }
}