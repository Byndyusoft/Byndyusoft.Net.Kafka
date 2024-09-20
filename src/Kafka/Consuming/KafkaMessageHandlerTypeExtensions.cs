namespace Byndyusoft.Net.Kafka.Consuming
{
    using System;
    using System.Linq;
    using Abstractions.Consuming;
    using CaseExtensions;
    using Infrastructure;

    internal static class KafkaMessageHandlerTypeExtensions
    {
        public static string BuildConsumersGroupId(this Type messageHandlerType, string solutionName)
        {
            var (projectName, serviceName) = solutionName.ExtractProjectAndServiceNames();
            var topic = string.Join("_", messageHandlerType.GetTopic().Split('.').Skip(1).Select(x => x.ToSnakeCase()));

            return $"{projectName}.{serviceName}.{topic}".ToLower();
        }
    }
}