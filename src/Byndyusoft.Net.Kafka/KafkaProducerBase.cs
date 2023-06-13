using System;
using System.Threading.Tasks;
using CaseExtensions;
using KafkaFlow.Producers;

namespace Byndyusoft.Net.Kafka
{
    /// <summary>
    ///     Produce T messages to kafka
    /// </summary>
    public abstract class KafkaProducerBase<T> : IKafkaProducer<T>
    {
        private readonly IProducerAccessor _producers;

        protected KafkaProducerBase(IProducerAccessor producers, string title)
        {
            _producers = producers ?? throw new ArgumentNullException(nameof(producers));
            if (string.IsNullOrEmpty(title))
                throw new ArgumentNullException(nameof(title));
            Title = title.ToSnakeCase();
        }

        public string Title { get; }
        public abstract string Topic { get; }
        public abstract string ClientName { get; }
        public abstract string KeyGenerator(T message);
        public Task ProduceAsync(T message) => _producers[Title].ProduceAsync(KeyGenerator(message), message);
    }
}