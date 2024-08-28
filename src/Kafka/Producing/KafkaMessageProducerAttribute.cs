namespace Byndyusoft.Net.Kafka.Producing
{
    using System;

    [AttributeUsage(AttributeTargets.Class)]
    public class KafkaMessageProducerAttribute : Attribute 
    {
        public string Topic { get; }

        public KafkaMessageProducerAttribute(string topic)
        {
            Topic = topic ?? throw new ArgumentNullException(nameof(topic));
        }
    }
}