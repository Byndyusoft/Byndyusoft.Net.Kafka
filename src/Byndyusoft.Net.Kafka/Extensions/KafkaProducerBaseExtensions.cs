namespace Byndyusoft.Net.Kafka.Extensions;

using CaseExtensions;
using System.Linq;

public static class KafkaProducerBaseExtensions
{
    public static string BuildClientId(this IKafkaProducer producer, string solutionName)
    {
        var solutionNameParts = solutionName.Split('.').ToArray();
        var project = solutionNameParts[0];
        var service = string.Join("_", solutionNameParts.Skip(1).Select(x => x.ToSnakeCase()));

        return $"{project}.{service}.{producer.Topic.Replace(".", "_")}".ToLower();
    }
}