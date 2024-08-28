namespace Byndyusoft.Net.Kafka.Producing
{
    using System.Threading.Tasks;

    /// <summary>
    /// Kafka producer contract
    /// </summary>
    public interface IKafkaProducer<in TMessage> : IKafkaProducer
    {
        /// <summary>
        /// Produces message to Kafka
        /// </summary>
        public Task ProduceAsync(TMessage message);
    }

    /// <summary>
    /// Producers marker interface
    /// </summary>
    public interface IKafkaProducer
    {
    }
}