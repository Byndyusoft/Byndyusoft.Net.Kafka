namespace Byndyusoft.Net.Kafka.Producing
{
    using System;
    using System.Linq;
    using Abstractions.Producing;
    using CaseExtensions;

    internal static class KafkaMessageProducerTypeExtensions
    {
        public static string GetProducingProfileName(this Type producerType) => producerType.FullName.ToSnakeCase();

        public static string BuildClientId(this Type producerType, string solutionName)
        {
            var solutionNameParts = solutionName.Split('.').ToArray();
            var project = solutionNameParts[1];
            var service = string.Join("_", solutionNameParts.Skip(2).Select(x => x.ToSnakeCase()));

            return $"{project}.{service}.{producerType.GetTopic().Replace(".", "_")}".ToLower();
        }
    }
}