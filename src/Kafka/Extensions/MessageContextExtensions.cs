using KafkaFlow;

namespace Byndyusoft.Net.Kafka.Extensions
{
    internal static class MessageContextExtensions
    {
        public static bool HasProducerContext(this IMessageContext messageContext)
            => messageContext.ProducerContext != null;

        public static bool HasConsumerContext(this IMessageContext messageContext)
            => messageContext.ConsumerContext != null;
    }
}