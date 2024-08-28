namespace Byndyusoft.Net.Kafka.Abstractions.Consuming
{
    using System;
    using System.Reflection;

    public static class KafkaMessageHandlerTypeExtensions
    {
        public static string GetTopic(this Type messageHandlerType)
            => messageHandlerType.GetCustomAttribute<KafkaMessageHandlerAttribute>(false)!.Topic;
    }
}