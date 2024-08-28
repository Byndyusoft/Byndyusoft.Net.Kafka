namespace Byndyusoft.Net.Kafka.Extensions
{
    using System;
    using System.Collections.Generic;
    using Producing;
    using Confluent.Kafka;
    using KafkaFlow;
    using KafkaFlow.Configuration;
    using KafkaFlow.Retry;
    using KafkaFlow.Serializer;
    using Middlewares;
    using Acks = Confluent.Kafka.Acks;
    using AutoOffsetReset = KafkaFlow.AutoOffsetReset;

    internal static class ClusterConfigurationBuilderExtensions
    {
        private const int MessageMaxSizeBytes = 40 * 1024 * 1024;

        private static ProducerConfig CreateProducerConfig(string solutionName, Type producerType)
            => new()
            {
                ClientId = producerType.BuildClientId(solutionName),
                Acks = Acks.All,
                EnableIdempotence = true,
                MaxInFlight = 1,
                MessageMaxBytes = MessageMaxSizeBytes,
                RetryBackoffMs = (int) TimeSpan.FromSeconds(1).TotalMilliseconds,
                CompressionType = CompressionType.Zstd
            };

        public static IClusterConfigurationBuilder AddProducers(
            this IClusterConfigurationBuilder clusterConfigurationBuilder,
            string solutionName,
            IEnumerable<Type> producerTypes
        )
        {
            foreach (var producerType in producerTypes)
                clusterConfigurationBuilder.AddProducer(
                    producerType.GetTitle(),
                    producerConfigurationBuilder => producerConfigurationBuilder
                        .DefaultTopic(producerType.GetTopic())
                        .WithProducerConfig(CreateProducerConfig(solutionName, producerType))
                        .AddMiddlewares(
                            pipeline => pipeline
                                .Add<ErrorsLoggingMiddleware>()
                                .AddSerializer(_ => new NewtonsoftJsonSerializer(JsonSerializerSettingsExtensions.DefaultSettings))
                                .Add<ProducedMessageLoggingMiddleware>()
                        )
                );

            return clusterConfigurationBuilder;
        }

        public static IClusterConfigurationBuilder AddConsumers(
            this IClusterConfigurationBuilder clusterConfigurationBuilder,
            string solutionName,
            IEnumerable<IKafkaConsumer> consumers
        )
        {
            foreach (var consumer in consumers)
                clusterConfigurationBuilder.AddConsumer(
                    consumerConfigurationBuilder => consumerConfigurationBuilder
                        .Topic(consumer.Topic)
                        .WithGroupId(consumer.BuildConsumersGroupId(solutionName))
                        .WithBufferSize(100)
                        .WithWorkersCount(10)
                        .WithAutoOffsetReset(AutoOffsetReset.Earliest)
                        .WithConsumerConfig(new ConsumerConfig {MaxPartitionFetchBytes = MessageMaxSizeBytes})
                        .AddMiddlewares(
                            pipeline => pipeline
                                .Add<ErrorsLoggingMiddleware>()
                                .Add<ConsumedMessageLoggingMiddleware>()
                                .AddDeserializer(_ => new NewtonsoftJsonDeserializer(JsonSerializerSettingsExtensions.DefaultSettings))
                                .AddTypedHandlers(h => h.AddHandlers(new[] {consumer.MessageHandler.GetType()}))
                                .RetrySimple(
                                    config => config
                                        .TryTimes(3)
                                        .WithTimeBetweenTriesPlan(retryNumber => TimeSpan.FromSeconds(Math.Pow(2, retryNumber)))
                                )
                        )
                );

            return clusterConfigurationBuilder;
        }
    }
}