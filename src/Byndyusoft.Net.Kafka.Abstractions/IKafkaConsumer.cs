using KafkaFlow.TypedHandler;

namespace Byndyusoft.Net.Kafka.Abstractions
{
    /// <summary>
    ///     Kafka messages consumer
    /// </summary>
    public interface IKafkaConsumer
    {
        /// <summary>
        ///     Kafka topic name
        /// </summary>
        public string Topic { get; }
        
        /// <summary>
        ///     Consumer group name
        /// </summary>
        public string GroupName { get; }
        
        /// <summary>
        ///     Message handling method
        /// </summary>
        public IMessageHandler MessageHandler { get; }
    }
}