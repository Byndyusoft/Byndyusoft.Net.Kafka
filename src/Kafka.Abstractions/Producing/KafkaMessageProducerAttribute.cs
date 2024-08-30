namespace Byndyusoft.Net.Kafka.Abstractions.Producing
{
    using System;

    [AttributeUsage(AttributeTargets.Class)]
    public class KafkaMessageProducerAttribute : Attribute 
    {
        public string Topic { get; }

        public string? ProducingProfileName { get; set; }

        public KafkaMessageProducerAttribute(string topic)
        {
            Topic = topic ?? throw new ArgumentNullException(nameof(topic));
        }
    }
}