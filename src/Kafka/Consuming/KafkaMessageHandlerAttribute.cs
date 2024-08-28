namespace Byndyusoft.Net.Kafka.Consuming
{
    using System;

    [AttributeUsage(AttributeTargets.Class)]
    public class KafkaMessageHandlerAttribute : Attribute
    {
        public string Topic { get; }

        public KafkaMessageHandlerAttribute(string topic)
        {
            Topic = topic ?? throw new ArgumentNullException(nameof(topic));
        }
    }
}