﻿using System.Threading.Tasks;

namespace Byndyusoft.Net.Kafka
{
    /// <summary>
    ///     TODO
    /// </summary>
    public interface IKafkaProducer
    {
        /// <summary>
        ///     TODO
        /// </summary>
        public string Title { get; }
        /// <summary>
        ///     TODO
        /// </summary>
        public string Topic { get; }
    }

    /// <summary>
    ///     TODO
    /// </summary>
    public interface IKafkaProducer<in T> : IKafkaProducer
    {
        /// <summary>
        ///     TODO
        /// </summary>
        public string KeyGenerator(T message);
        /// <summary>
        ///     TODO
        /// </summary>
        public Task ProduceAsync(T message);
    }
}