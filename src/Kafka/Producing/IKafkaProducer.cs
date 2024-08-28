namespace Byndyusoft.Net.Kafka.Producing
{
    using System.Threading.Tasks;

    /// <summary>
    ///  Produce messages to Kafka
    /// </summary>
    public interface IKafkaProducer<in TMessage>
    {
        /// <summary>
        /// Push message to queue
        /// </summary>
        public Task ProduceAsync(TMessage message);
    }
}