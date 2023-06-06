using System.Linq;
using CaseExtensions;

namespace Byndyusoft.Net.Kafka.Extensions
{
    internal static class KafkaConsumerBaseExtensions
    {
        //TODO Add Prefix
        public static string BuildConsumerGroupId(this IKafkaConsumer consumer, string solutionName)
        {
            var arr = solutionName.Split('.', '_');
            return $"{string.Join("_", arr.Skip(2).Take(arr.Length - 2).ToArray()).ToSnakeCase()}.{consumer.Topic.Replace(".", "_")}";
        }
    }
}