namespace Byndyusoft.Net.Kafka.Consuming
{
    using KafkaFlow;

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
        ///     Message handling method
        /// </summary>
        public IMessageHandler MessageHandler { get; }
    }
}