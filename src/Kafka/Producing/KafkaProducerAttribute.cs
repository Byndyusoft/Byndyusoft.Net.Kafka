namespace Byndyusoft.Net.Kafka.Producing
{
    using System;

    [AttributeUsage(AttributeTargets.Class)]
    public class KafkaProducerAttribute : Attribute 
    {
        public string Topic { get; }

        public KafkaProducerAttribute(string topic)
        {
            Topic = topic ?? throw new ArgumentNullException(nameof(topic));
        }
    }
}