namespace Byndyusoft.Net.Kafka.Producing
{
    using System;
    using KafkaFlow.Producers;
    using System.Threading.Tasks;
    using Abstractions.Producing;

    internal class KafkaMessageSender : IKafkaMessageSender
    {
        private readonly IProducerAccessor _producerAccessor;

        public KafkaMessageSender(IProducerAccessor producerAccessor)
        {
            _producerAccessor = producerAccessor ?? throw new ArgumentNullException(nameof(producerAccessor));
        }

        public Task SendAsync<TMessage>(string producingProfileName, string messageKey, TMessage message)
            => _producerAccessor[producingProfileName].ProduceAsync(messageKey, message);
    }
}