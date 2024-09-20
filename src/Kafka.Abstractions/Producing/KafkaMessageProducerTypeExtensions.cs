namespace Byndyusoft.Net.Kafka.Abstractions.Producing
{
    using System;
    using System.Reflection;

    public static class KafkaMessageProducerTypeExtensions
    {
        public static string GetTopic(this Type producerType) => producerType.GetCustomAttribute<KafkaMessageProducerAttribute>(false)!.Topic;
    }
}