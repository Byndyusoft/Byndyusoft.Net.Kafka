﻿namespace Byndyusoft.Net.Kafka.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Abstractions.Consuming;
    using Abstractions.Producing;
    using Consuming;
    using Logging;
    using Producing;
    using KafkaFlow;
    using KafkaFlow.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceCollectionExtensions
    {
        private static Type? GetMessageType(this Type type, Type requiredBaseType)
        {
            for (var baseType = type; baseType != null; baseType = baseType.BaseType)
                if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == requiredBaseType)
                    return baseType.GetGenericArguments().Single();

            return null;
        }

        private static IEnumerable<Type> GetServiceTypes(this Assembly assembly, Type requiredBaseType, Type requiredAttributeType)
            => assembly.GetTypes()
                .Where(type => type is {IsPublic: true, IsClass: true, IsAbstract: false, IsGenericType: false})
                .Where(type => type.GetMessageType(requiredBaseType) != null)
                .Where(type => type.GetCustomAttribute(requiredAttributeType, false) != null);

        private static Type[] GetMessageProducerTypes(IEnumerable<Assembly> assemblies)
        {
            var requiredBaseType = typeof(KafkaMessageProducerBase<>);
            var requiredAttributeType = typeof(KafkaMessageProducerAttribute);
            return assemblies
                .SelectMany(assembly => assembly.GetServiceTypes(requiredBaseType, requiredAttributeType))
                .ToArray();
        }

        private static Type[] GetMessageHandlerTypes(IEnumerable<Assembly> assemblies)
        {
            var requiredBaseType = typeof(KafkaMessageHandlerBase<>);
            var requiredAttributeType = typeof(KafkaMessageHandlerAttribute);
            return assemblies
                .SelectMany(assembly => assembly.GetServiceTypes(requiredBaseType, requiredAttributeType))
                .ToArray();
        }

        private static IServiceCollection AddMessageProducers(this IServiceCollection services, IEnumerable<Type> producerTypes)
        {
            var markerInterfaceType = typeof(IKafkaMessageProducer);
            var interfaceType = typeof(IKafkaMessageProducer<>);
            var baseType = typeof(KafkaMessageProducerBase<>);
            foreach (var producerType in producerTypes)
                services
                    .AddSingleton(producerType)
                    .AddSingleton(markerInterfaceType, producerType)
                    .AddSingleton(interfaceType.MakeGenericType(producerType.GetMessageType(baseType)), producerType);

            return services;
        }

        private static IServiceCollection AddMessageHandlers(this IServiceCollection services, IEnumerable<Type> handlerTypes)
        {
            var markerInterfaceType = typeof(IKafkaMessageHandler);
            foreach (var handlerType in handlerTypes)
                services
                    .AddSingleton(handlerType)
                    .AddSingleton(markerInterfaceType, handlerType);

            return services;
        }

        /// <summary>
        /// Register Kafka in DI
        /// </summary>
        public static IServiceCollection AddKafkaBus(this IServiceCollection services, KafkaSettings kafkaSettings)
        {
            var callingAssembly = Assembly.GetCallingAssembly();
            var callingAssemblyName = callingAssembly.GetName().Name!;

            var assemblies = callingAssembly.LoadReferencedAssemblies().ToArray();
            var producerTypes = GetMessageProducerTypes(assemblies);
            var messageHandlerTypes = GetMessageHandlerTypes(assemblies);
            return services
                .AddSingleton<IKafkaMessageSender, KafkaMessageSender>()
                .AddMessageProducers(producerTypes)
                .AddMessageHandlers(messageHandlerTypes)
                .AddKafka(
                    kafka => kafka
                        .UseLogHandler<LoggerHandler>()
                        .AddOpenTelemetryInstrumentation()
                        .AddCluster(
                            cluster =>
                                {
                                    cluster
                                        .WithBrokers(kafkaSettings.Hosts)
                                        .WithSecurityInformation(
                                            information =>
                                                {
                                                    information.SaslMechanism = SaslMechanism.ScramSha512;
                                                    information.SecurityProtocol = SecurityProtocol.SaslPlaintext;
                                                    information.SaslUsername = kafkaSettings.Username;
                                                    information.SaslPassword = kafkaSettings.Password;
                                                }
                                        )
                                        .AddProducers(callingAssemblyName, producerTypes)
                                        .AddConsumers(callingAssemblyName, messageHandlerTypes);
                                }
                        )
                );
        }
    }
}