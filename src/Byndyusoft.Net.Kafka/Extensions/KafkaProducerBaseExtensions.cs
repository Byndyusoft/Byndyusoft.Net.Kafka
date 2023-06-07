namespace Byndyusoft.Net.Kafka.Extensions
{
    internal static class KafkaProducerBaseExtensions
    {
        public static string BuildClientId(this IKafkaProducer producer, string prefix, string serviceName)
        {
            return $"{prefix}.{serviceName}.{producer.Topic.Replace(".", "_")}";
        }
    }
}