namespace Byndyusoft.Net.Kafka.Consuming
{
    using System.Linq;
    using CaseExtensions;

    public static class KafkaConsumerBaseExtensions
    {
        public static string BuildConsumersGroupId(this IKafkaConsumer consumer, string solutionName)
        {
            var solutionNameParts = solutionName.Split('.').ToArray();
            var project = solutionNameParts[1];
            var service = string.Join("_", solutionNameParts.Skip(2).Select(x => x.ToSnakeCase()));

            return $"{project}.{service}.{consumer.Topic.Replace(".", "_")}".ToLower();
        }
    }
}