using System;
using System.Collections.Generic;
using Byndyusoft.Net.Kafka.Middlewares;
using Confluent.Kafka;
using KafkaFlow;
using KafkaFlow.Configuration;
using KafkaFlow.Retry;
using KafkaFlow.Serializer;
using KafkaFlow.TypedHandler;
using Newtonsoft.Json;

namespace Byndyusoft.Net.Kafka.Extensions
{
    using Acks = Confluent.Kafka.Acks;
    using AutoOffsetReset = KafkaFlow.AutoOffsetReset;

    public static class ClusterConfigurationBuilderExtensions
    {
        private const int MessageMaxSizeBytes = 20 * 1024 * 1024;

        private static ProducerConfig CreateProducerConfig(IKafkaProducer producer, string solutionName)
        {
            return new ProducerConfig
                   {
                       ClientId = producer.BuildClientId(solutionName),
                       Acks = Acks.All,
                       EnableIdempotence = true,
                       MaxInFlight = 1,
                       MessageSendMaxRetries = 3,
                       MessageMaxBytes = MessageMaxSizeBytes,
                       RetryBackoffMs = (int) TimeSpan.FromSeconds(1).TotalMilliseconds
                   };
        }

        public static IClusterConfigurationBuilder AddProducers(
            this IClusterConfigurationBuilder clusterConfigurationBuilder,
            IEnumerable<IKafkaProducer> producers,
            string solutionName
        )
        {
            foreach (var producer in producers)
                clusterConfigurationBuilder.AddProducer(
                    producer.Title,
                    producerConfigurationBuilder => producerConfigurationBuilder
                        .DefaultTopic(producer.Topic)
                        .WithProducerConfig(CreateProducerConfig(producer, solutionName))
                        .AddMiddlewares(
                            middlewares => middlewares
                                .Add<PublishMessageTracingMiddleware>()
                                .Add<ErrorHandlingMiddleware>()
                                .AddSerializer(
                                    x => new NewtonsoftJsonSerializer(new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto })
                                )
                        )
                );
            return clusterConfigurationBuilder;
        }

        public static IClusterConfigurationBuilder AddConsumers(
            this IClusterConfigurationBuilder clusterConfigurationBuilder,
            IEnumerable<IKafkaConsumer> consumers,
            string solutionName
        )
        {
            foreach (var consumer in consumers)
                clusterConfigurationBuilder.AddConsumer(
                    consumerConfigurationBuilder => consumerConfigurationBuilder
                        .Topic(consumer.Topic)
                        .WithGroupId(consumer.BuildConsumerGroupId(solutionName))
                        .WithBufferSize(100)
                        .WithWorkersCount(10)
                        .WithAutoOffsetReset(AutoOffsetReset.Earliest)
                        .WithConsumerConfig(new ConsumerConfig {MaxPartitionFetchBytes = MessageMaxSizeBytes})
                        .AddMiddlewares(
                            middlewares =>
                                middlewares
                                    .Add<ConsumeMessageTracingMiddleware>()
                                    .Add<ErrorHandlingMiddleware>()
                                    .AddSerializer(
                                        x => new NewtonsoftJsonSerializer(
                                            new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.Auto}
                                        )
                                    )
                                    .AddTypedHandlers(h => h.AddHandlers(new[] {consumer.MessageHandler.GetType()}))
                                    .RetrySimple(
                                        config => config
                                            .TryTimes(3)
                                            .WithTimeBetweenTriesPlan(
                                                retryCount => TimeSpan.FromMilliseconds(Math.Pow(2, retryCount) * 1000)
                                            )
                                    )
                        )
                );

            return clusterConfigurationBuilder;
        }
    }
}