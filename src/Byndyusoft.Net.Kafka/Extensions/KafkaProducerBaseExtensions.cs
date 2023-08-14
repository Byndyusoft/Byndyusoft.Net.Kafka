using CaseExtensions;

namespace Byndyusoft.Net.Kafka.Extensions
{
    internal static class KafkaProducerBaseExtensions
    {
        public static string BuildProducerClientId(this IKafkaProducer producer, string prefix)
            => $"{prefix.ToSnakeCase()}.{producer.Topic.Replace(".", "_")}";
    }
}