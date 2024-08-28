namespace Byndyusoft.Net.Kafka.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Consuming;
    using Logging;
    using Producing;
    using KafkaFlow;
    using KafkaFlow.Configuration;
    using Microsoft.Extensions.Configuration;
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

        private static IServiceCollection AddMessageProducers(
            this IServiceCollection services,
            IEnumerable<Type> producerTypes
        )
        {
            var producersMarkerInterfaceType = typeof(IKafkaMessageProducer);
            var producerInterfaceType = typeof(IKafkaMessageProducer<>);
            var producerBaseType = typeof(KafkaMessageProducerBase<>);
            foreach (var producerType in producerTypes)
                services
                    .AddSingleton(producerType)
                    .AddSingleton(producersMarkerInterfaceType, producerType)
                    .AddSingleton(producerInterfaceType.MakeGenericType(producerType.GetMessageType(producerBaseType)), producerType);

            return services;
        }

        private static IServiceCollection AddMessageHandlers(
            this IServiceCollection services,
            IEnumerable<Type> messageHandlerTypes
        )
        {
            var messageHandlersMarkerInterfaceType = typeof(IKafkaMessageHandler);
            foreach (var messageHandlerType in messageHandlerTypes)
                services
                    .AddSingleton(messageHandlerType)
                    .AddSingleton(messageHandlersMarkerInterfaceType, messageHandlerType);

            return services;
        }

        /// <summary>
        /// Register Kafka in DI
        /// </summary>
        public static IServiceCollection AddKafkaBus(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            var kafkaSettings = configuration.GetSection(nameof(KafkaSettings)).Get<KafkaSettings>();

            var callingAssembly = Assembly.GetCallingAssembly();
            var callingAssemblyName = callingAssembly.GetName().Name!;

            var assemblies = callingAssembly.LoadReferencedAssemblies().ToArray();
            var producerTypes = GetMessageProducerTypes(assemblies);
            var messageHandlerTypes = GetMessageHandlerTypes(assemblies);
            return services
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