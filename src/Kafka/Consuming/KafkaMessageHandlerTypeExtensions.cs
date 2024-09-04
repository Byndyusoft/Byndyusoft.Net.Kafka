namespace Byndyusoft.Net.Kafka.Consuming
{
    using System;
    using System.Linq;
    using Abstractions.Consuming;
    using CaseExtensions;

    internal static class KafkaMessageHandlerTypeExtensions
    {
        public static string BuildConsumersGroupId(this Type messageHandlerType, string solutionName)
        {
            var solutionNameParts = solutionName.Split('.').ToArray();
            var project = solutionNameParts[1].ToSnakeCase();
            var service = string.Join("_", solutionNameParts.Skip(2).Select(x => x.ToSnakeCase()));

            var topic = string.Join("_", messageHandlerType.GetTopic().Split('.').Skip(1).Select(x => x.ToSnakeCase()));

            return $"{project}.{service}.{topic}".ToLower();
        }
    }
}