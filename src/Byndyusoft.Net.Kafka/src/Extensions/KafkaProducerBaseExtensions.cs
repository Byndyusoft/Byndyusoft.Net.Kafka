using System.Linq;
using CaseExtensions;

namespace Byndyusoft.Net.Kafka.Extensions
{
    internal static class KafkaProducerBaseExtensions
    {
        //TODO Add Prefix
        public static string BuildClientId(this IKafkaProducer producer, string solutionName)
        {
            var arr = solutionName.Split('.', '_');
            return $"{string.Join("_", arr.Skip(2).Take(arr.Length - 2).ToArray()).ToSnakeCase()}.{producer.Topic.Replace(".", "_")}";
        }
    }
}