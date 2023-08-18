﻿namespace Byndyusoft.Net.Kafka
{
    using System.Threading.Tasks;

    /// <summary>
    ///     Produce messages to kafka
    /// </summary>
    public interface IKafkaProducer
    {
        /// <summary>
        ///     Kafka title name
        /// </summary>
        public string Title { get; }

        /// <summary>
        ///     Kafka topic name
        /// </summary>
        public string Topic { get; }
    }

    /// <summary>
    ///     Produce T messages to kafka
    /// </summary>
    public interface IKafkaProducer<in T> : IKafkaProducer
    {
        /// <summary>
        ///     Generate key
        /// </summary>
        public string KeyGenerator(T message);

        /// <summary>
        ///     Push message to queue
        /// </summary>
        public Task ProduceAsync(T message);
    }
}