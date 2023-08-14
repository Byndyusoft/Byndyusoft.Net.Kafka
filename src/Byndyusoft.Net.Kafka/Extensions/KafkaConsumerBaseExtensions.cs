namespace Byndyusoft.Net.Kafka.Extensions
{
    using CaseExtensions;

    internal static class KafkaConsumerBaseExtensions
    {
        public static string BuildConsumersGroupId(this IKafkaConsumer consumer, string prefix)
            => $"{prefix.ToSnakeCase()}.{consumer.Topic.Replace(".", "_")}";
    }
}