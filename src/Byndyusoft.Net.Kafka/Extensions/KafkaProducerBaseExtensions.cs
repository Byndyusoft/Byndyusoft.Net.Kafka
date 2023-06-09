namespace Byndyusoft.Net.Kafka.Extensions
{
    internal static class KafkaProducerBaseExtensions
    {
        public static string BuildProducerClientId(this IKafkaProducer producer, string prefix, string clientName)
        {
            return $"{prefix}.{clientName}.{producer.Topic.Replace(".", "_")}";
        }
    }
}