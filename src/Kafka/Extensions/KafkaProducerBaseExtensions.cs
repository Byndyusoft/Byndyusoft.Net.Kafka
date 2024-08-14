namespace Byndyusoft.Net.Kafka.Extensions
{
    using System.Linq;
    using CaseExtensions;

    public static class KafkaProducerBaseExtensions
    {
        public static string BuildClientId(this IKafkaProducer producer, string solutionName)
        {
            var solutionNameParts = solutionName.Split('.').ToArray();
            var project = solutionNameParts[1];
            var service = string.Join("_", solutionNameParts.Skip(2).Select(x => x.ToSnakeCase()));

            return $"{project}.{service}.{producer.Topic.Replace(".", "_")}".ToLower();
        }
    }
}