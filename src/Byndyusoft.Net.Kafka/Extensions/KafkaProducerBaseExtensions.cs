using Byndyusoft.Net.Kafka.Abstractions;

namespace Byndyusoft.Net.Kafka.Extensions
{
    internal static class KafkaProducerBaseExtensions
    {
        public static string BuildProducerClientId(this IKafkaProducer producer, string prefix)
        {
            return $"{prefix}.{producer.ClientName}.{producer.Topic.Replace(".", "_")}";
        }
    }
}