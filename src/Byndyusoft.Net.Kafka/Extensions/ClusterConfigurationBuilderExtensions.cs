using System;
using System.Collections.Generic;
using Byndyusoft.Net.Kafka.Abstractions;
using Byndyusoft.Net.Kafka.Middlewares;
using Confluent.Kafka;
using KafkaFlow;
using KafkaFlow.Configuration;
using KafkaFlow.Retry;
using KafkaFlow.Serializer;
using KafkaFlow.TypedHandler;
using Newtonsoft.Json;
using Acks = Confluent.Kafka.Acks;
using AutoOffsetReset = KafkaFlow.AutoOffsetReset;

namespace Byndyusoft.Net.Kafka.Extensions
{
    internal static class ClusterConfigurationBuilderExtensions
    {
        //TODO по необходимости вынести это в настройки https://github.com/Byndyusoft/Byndyusoft.Net.Kafka/issues/5
        private const int MessageMaxSizeBytes = 20 * 1024 * 1024;
        private const int BufferSize = 100;
        private const int WorkersCount = 10;
        private const int TryCount = 3;
        private const int MaxInFlight = 1;
        private const int RetryBackoffS = 1;

        public static IClusterConfigurationBuilder AddProducers(
            this IClusterConfigurationBuilder clusterConfigurationBuilder,
            IEnumerable<IKafkaProducer> producers, 
            string prefix
        )
        {
            foreach (var producer in producers)
                clusterConfigurationBuilder = clusterConfigurationBuilder.AddProducer(
                    producer.Title,
                    producerConfigurationBuilder => producerConfigurationBuilder
                        .DefaultTopic(producer.Topic)
                        .WithProducerConfig(CreateProducerConfig(producer, prefix))
                        .AddMiddlewares(
                            middlewares => middlewares
                                .Add<PublishMessageTracingMiddleware>()
                                .Add<ErrorHandlingMiddleware>()
                                .AddSerializer(
                                    _ => new NewtonsoftJsonSerializer(new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto })
                                )
                        )
                );
            
            return clusterConfigurationBuilder;
        }

        public static IClusterConfigurationBuilder AddConsumers(
            this IClusterConfigurationBuilder clusterConfigurationBuilder,
            IEnumerable<IKafkaConsumer> consumers, 
            string prefix
        )
        {
            foreach (var consumer in consumers)
                clusterConfigurationBuilder = clusterConfigurationBuilder.AddConsumer(
                    consumerConfigurationBuilder => consumerConfigurationBuilder
                        .Topic(consumer.Topic)
                        .WithGroupId(consumer.BuildConsumerGroupId(prefix))
                        .WithBufferSize(BufferSize)
                        .WithWorkersCount(WorkersCount)
                        .WithAutoOffsetReset(AutoOffsetReset.Earliest)
                        .WithConsumerConfig(new ConsumerConfig {MaxPartitionFetchBytes = MessageMaxSizeBytes})
                        .AddMiddlewares(
                            middlewares =>
                                middlewares
                                    .Add<ConsumeMessageTracingMiddleware>()
                                    .Add<ErrorHandlingMiddleware>()
                                    .AddSerializer(
                                        _ => new NewtonsoftJsonSerializer(
                                            new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.Auto}
                                        )
                                    )
                                    .AddTypedHandlers(h => h.AddHandlers(new[] {consumer.MessageHandler.GetType()}))
                                    .RetrySimple(
                                        config => config
                                            .TryTimes(TryCount)
                                            .WithTimeBetweenTriesPlan(CalculateTimeBetweenTries)
                                    )
                        )
                );

            return clusterConfigurationBuilder;
        }

        private static ProducerConfig CreateProducerConfig(IKafkaProducer producer, string prefix)
        {
            return new ProducerConfig
            {
                ClientId = producer.BuildProducerClientId(prefix),
                Acks = Acks.All,
                EnableIdempotence = true,
                MaxInFlight = MaxInFlight,
                MessageSendMaxRetries = TryCount,
                MessageMaxBytes = MessageMaxSizeBytes,
                RetryBackoffMs = (int) TimeSpan.FromSeconds(RetryBackoffS).TotalMilliseconds 
            };
        }

        private static TimeSpan CalculateTimeBetweenTries(int retryCount)
        {
            return TimeSpan.FromSeconds(Math.Pow(2, retryCount));
        }
    }
}