namespace Byndyusoft.Net.Kafka.Consuming
{
    using System;
    using System.Linq;
    using System.Reflection;
    using CaseExtensions;

    internal static class KafkaMessageHandlerTypeExtensions
    {
        public static string GetTopic(this Type messageHandlerType)
            => messageHandlerType.GetCustomAttribute<KafkaMessageHandlerAttribute>(false)!.Topic;

        public static string BuildConsumersGroupId(this Type messageHandlerType, string solutionName)
        {
            var solutionNameParts = solutionName.Split('.').ToArray();
            var project = solutionNameParts[1];
            var service = string.Join("_", solutionNameParts.Skip(2).Select(x => x.ToSnakeCase()));

            return $"{project}.{service}.{messageHandlerType.GetTopic().Replace(".", "_")}".ToLower();
        }
    }
}