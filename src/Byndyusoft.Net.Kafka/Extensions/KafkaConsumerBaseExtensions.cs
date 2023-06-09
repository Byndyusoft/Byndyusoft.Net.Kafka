﻿namespace Byndyusoft.Net.Kafka.Extensions
{
    internal static class KafkaConsumerBaseExtensions
    {
        public static string BuildConsumerGroupId(this IKafkaConsumer consumer, string prefix, string groupName)
        {
            return $"{prefix}.{groupName}.{consumer.Topic.Replace(".", "_")}";
        }
    }
}