namespace Byndyusoft.Net.Kafka.Producing
{
    using System.Threading.Tasks;

    /// <summary>
    /// Kafka producer contract
    /// </summary>
    public interface IKafkaMessageProducer<in TMessage> : IKafkaMessageProducer
    {
        /// <summary>
        /// Produces message to Kafka
        /// </summary>
        public Task ProduceAsync(TMessage message);
    }

    /// <summary>
    /// Producers marker interface
    /// </summary>
    public interface IKafkaMessageProducer
    {
    }
}