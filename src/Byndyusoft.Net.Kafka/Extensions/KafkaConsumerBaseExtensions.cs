using Byndyusoft.Net.Kafka.Abstractions;

namespace Byndyusoft.Net.Kafka.Extensions
{
    internal static class KafkaConsumerBaseExtensions
    {
        public static string BuildConsumerGroupId(this IKafkaConsumer consumer, string prefix)
        {
            return $"{prefix}.{consumer.GroupName}.{consumer.Topic.Replace(".", "_")}";
        }
    }
}