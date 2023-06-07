namespace Byndyusoft.Net.Kafka.Extensions
{
    internal static class KafkaConsumerBaseExtensions
    {
        public static string BuildConsumerGroupId(this IKafkaConsumer consumer, string prefix, string serviceName)
        {
            return $"{prefix}.{serviceName}.{consumer.Topic.Replace(".", "_")}";
        }
    }
}