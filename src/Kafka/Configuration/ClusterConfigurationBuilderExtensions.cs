namespace Byndyusoft.Net.Kafka.Configuration
{
    using System;
    using System.Collections.Generic;
    using Consuming;
    using Logging;
    using Producing;
    using Confluent.Kafka;
    using KafkaFlow;
    using KafkaFlow.Configuration;
    using KafkaFlow.Serializer;
    using Acks = Confluent.Kafka.Acks;
    using AutoOffsetReset = KafkaFlow.AutoOffsetReset;
    using KafkaMessageProducerTypeExtensions = Abstractions.Producing.KafkaMessageProducerTypeExtensions;
    using KafkaMessageHandlerTypeExtensions = Abstractions.Consuming.KafkaMessageHandlerTypeExtensions;

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
            IEnumerable<Type> messageProducerTypes
        )
        {
            foreach (var messageProducerType in messageProducerTypes)
                clusterConfigurationBuilder
                    .AddProducer(
                        messageProducerType.GetProducingProfileName(),
                        producerConfigurationBuilder => producerConfigurationBuilder
                            .DefaultTopic(KafkaMessageProducerTypeExtensions.GetTopic(messageProducerType))
                            .WithProducerConfig(CreateProducerConfig(solutionName, messageProducerType))
                            .AddMiddlewares(
                                pipeline => pipeline
                                    .Add<ErrorsLoggingMiddleware>()
                                    .Add<ProducedMessageLoggingMiddleware>()
                                    .AddSerializer(_ => new NewtonsoftJsonSerializer(JsonSerializerSettingsExtensions.DefaultSettings))
                            )
                    );

            return clusterConfigurationBuilder;
        }

        public static IClusterConfigurationBuilder AddConsumers(
            this IClusterConfigurationBuilder clusterConfigurationBuilder,
            string solutionName,
            IEnumerable<Type> messageHandlerTypes
        )
        {
            foreach (var messageHandlerType in messageHandlerTypes)
                clusterConfigurationBuilder
                    .AddConsumer(
                        consumerConfigurationBuilder => consumerConfigurationBuilder
                            .Topic(KafkaMessageHandlerTypeExtensions.GetTopic(messageHandlerType))
                            .WithGroupId(messageHandlerType.BuildConsumersGroupId(solutionName))
                            .WithBufferSize(100)
                            .WithWorkersCount(10)
                            .WithAutoOffsetReset(AutoOffsetReset.Earliest)
                            .WithConsumerConfig(new ConsumerConfig {MaxPartitionFetchBytes = MessageMaxSizeBytes})
                            .AddMiddlewares(
                                pipeline => pipeline
                                    .Add<ErrorsLoggingMiddleware>()
                                    .AddDeserializer(_ => new NewtonsoftJsonDeserializer(JsonSerializerSettingsExtensions.DefaultSettings))
                                    .Add<ConsumedMessageLoggingMiddleware>()
                                    .AddTypedHandlers(h => h.AddHandlers(new[] {messageHandlerType}))
                            )
                    );

            return clusterConfigurationBuilder;
        }
    }
}