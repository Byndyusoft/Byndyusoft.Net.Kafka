namespace Byndyusoft.Net.Kafka.Extensions;

using System.Linq;
using CaseExtensions;

public static class KafkaConsumerBaseExtensions
{
    public static string BuildConsumersGroupId(this IKafkaConsumer consumer, string solutionName)
    {
        var solutionNameParts = solutionName.Split('.').ToArray();
        var project = solutionNameParts[0];
        var service = string.Join("_", solutionNameParts.Skip(1).Select(x => x.ToSnakeCase()));

        return $"{project}.{service}.{consumer.Topic.Replace(".", "_")}".ToLower();
    }
}