namespace Byndyusoft.Net.Kafka.Extensions;

using System;
using System.Collections.Generic;
using Confluent.Kafka;
using KafkaFlow;
using KafkaFlow.Configuration;
using KafkaFlow.Retry;
using KafkaFlow.Serializer;
using KafkaFlow.TypedHandler;
using Middlewares;
using Newtonsoft.Json;
using Acks = Confluent.Kafka.Acks;
using AutoOffsetReset = KafkaFlow.AutoOffsetReset;

internal static class ClusterConfigurationBuilderExtensions
{
    private const int MessageMaxSizeBytes = 20 * 1024 * 1024;

    public static IClusterConfigurationBuilder AddProducers(
        this IClusterConfigurationBuilder clusterConfigurationBuilder,
        IEnumerable<IKafkaProducer> producers,
        string prefix
    )
    {
        foreach (var producer in producers)
            clusterConfigurationBuilder.AddProducer(
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
            clusterConfigurationBuilder.AddConsumer(
                consumerConfigurationBuilder => consumerConfigurationBuilder
                    .Topic(consumer.Topic)
                    .WithGroupId(consumer.BuildConsumersGroupId(prefix))
                    .WithBufferSize(100)
                    .WithWorkersCount(10)
                    .WithAutoOffsetReset(AutoOffsetReset.Earliest)
                    .AddMiddlewares(
                        middlewares =>
                            middlewares
                                .Add<ConsumeMessageTracingMiddleware>()
                                .Add<ErrorHandlingMiddleware>()
                                .AddSerializer(
                                    _ => new NewtonsoftJsonSerializer(
                                        new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto }
                                    )
                                )
                                .AddTypedHandlers(h => h.AddHandlers(new[] { consumer.MessageHandler.GetType() }))
                                .RetrySimple(
                                    config => config
                                        .TryTimes(3)
                                        .WithTimeBetweenTriesPlan(retryNumber => TimeSpan.FromSeconds(Math.Pow(2, retryNumber)))
                                )
                    )
            );

        return clusterConfigurationBuilder;
    }

    private static ProducerConfig CreateProducerConfig(IKafkaProducer producer, string prefix)
    {
        return new ProducerConfig
               {
                   ClientId = producer.BuildClientId(prefix),
                   Acks = Acks.All,
                   EnableIdempotence = true,
                   MaxInFlight = 1,
                   MessageSendMaxRetries = 3,
                   MessageMaxBytes = MessageMaxSizeBytes,
                   RetryBackoffMs = (int)TimeSpan.FromSeconds(1).TotalMilliseconds
               };
    }
}