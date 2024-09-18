namespace Byndyusoft.Net.Kafka.Producing
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Abstractions.Producing;
    using CaseExtensions;
    using Infrastructure;

    internal static class KafkaMessageProducerTypeExtensions
    {
        public static string GetProducingProfileName(this Type producerType)
            => (producerType.GetCustomAttribute<KafkaMessageProducerAttribute>(false)!.ProducingProfileName ?? producerType.FullName).ToSnakeCase();

        public static string BuildClientId(this Type producerType, string solutionName)
        {
            var (projectName, serviceName) = solutionName.ExtractProjectAndServiceNames();
            var topic = string.Join("_", producerType.GetTopic().Split('.').Skip(1).Select(x => x.ToSnakeCase()));

            return $"{projectName}.{serviceName}.{topic}".ToLower();
        }
    }
}