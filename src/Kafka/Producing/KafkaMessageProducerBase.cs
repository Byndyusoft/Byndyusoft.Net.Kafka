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
        private readonly string _producingProfileName;
        private readonly IKafkaMessageSender _messageSender;

        protected KafkaMessageProducerBase(IKafkaMessageSender messageSender)
        {
            _producingProfileName = GetType().GetProducingProfileName();
            _messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
        }

        /// <summary>
        /// Generates key for each message
        /// </summary>
        /// <param name="message">Message</param>
        /// <returns>Generated key</returns>
        protected abstract string KeyGenerator(TMessage message);

        /// <inheritdoc />
        public Task ProduceAsync(TMessage message) => _messageSender.SendAsync(_producingProfileName, KeyGenerator(message), message);
    }
}