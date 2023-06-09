using KafkaFlow.TypedHandler;

namespace Byndyusoft.Net.Kafka
{
    /// <summary>
    ///     TODO
    /// </summary>
    public interface IKafkaConsumer
    {
        /// <summary>
        ///     TODO
        /// </summary>
        public string Topic { get; }
        /// <summary>
        ///     TODO
        /// </summary>
        public IMessageHandler MessageHandler { get; }
    }
}