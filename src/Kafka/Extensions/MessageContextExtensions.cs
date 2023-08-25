namespace Byndyusoft.Net.Kafka.Extensions;

using KafkaFlow;

internal static class MessageContextExtensions
{
    public static bool HasProducerContext(this IMessageContext messageContext)
    {
        return messageContext.ProducerContext != null;
    }

    public static bool HasConsumerContext(this IMessageContext messageContext)
    {
        return messageContext.ConsumerContext != null;
    }
}