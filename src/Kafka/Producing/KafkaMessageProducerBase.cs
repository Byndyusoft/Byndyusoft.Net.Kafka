namespace Byndyusoft.Net.Kafka.Producing
{
    using System;
    using System.Threading.Tasks;
    using Abstractions.Producing;
    using KafkaFlow.Producers;

    /// <summary>
    /// Produce <typeparamref name="TMessage"/> messages to Kafka
    /// </summary>
    public abstract class KafkaMessageProducerBase<TMessage> : IKafkaMessageProducer<TMessage>
    {
        private readonly string _title;
        private readonly IProducerAccessor _producers;

        protected KafkaMessageProducerBase(IProducerAccessor producers)
        {
            _title = GetType().GetTitle();
            _producers = producers ?? throw new ArgumentNullException(nameof(producers));
        }

        /// <summary>
        /// Generates key for each message
        /// </summary>
        /// <param name="message">Message</param>
        /// <returns>Generated key</returns>
        protected abstract string KeyGenerator(TMessage message);

        /// <inheritdoc />
        public Task ProduceAsync(TMessage message) => _producers[_title].ProduceAsync(KeyGenerator(message), message);
    }
}