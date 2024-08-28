namespace Byndyusoft.Net.Kafka.Producing
{
    using System;
    using System.Linq;
    using System.Reflection;
    using CaseExtensions;

    internal static class KafkaProducerTypeExtensions
    {
        public static string GetTitle(this Type producerType) => producerType.FullName.ToSnakeCase();

        public static string GetTopic(this Type producerType) => producerType.GetCustomAttribute<KafkaProducerAttribute>(false)!.Topic;

        public static string BuildClientId(this Type producerType, string solutionName)
        {
            var solutionNameParts = solutionName.Split('.').ToArray();
            var project = solutionNameParts[1];
            var service = string.Join("_", solutionNameParts.Skip(2).Select(x => x.ToSnakeCase()));

            return $"{project}.{service}.{producerType.GetTopic().Replace(".", "_")}".ToLower();
        }
    }
}