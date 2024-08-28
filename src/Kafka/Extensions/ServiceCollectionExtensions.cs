namespace Byndyusoft.Net.Kafka.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Consuming;
    using Handlers;
    using KafkaFlow;
    using KafkaFlow.Configuration;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Producing;

    public static class ServiceCollectionExtensions
    {
        private static Type? GetMessageType(this Type type, Type requiredBaseType)
        {
            for (var baseType = type; baseType != null; baseType = baseType.BaseType)
                if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == requiredBaseType)
                    return baseType.GetGenericArguments().Single();

            return null;
        }

        private static Type[] GetProducerTypes(IEnumerable<Assembly> assemblies)
        {
            var requiredBaseType = typeof(KafkaProducerBase<>);
            return assemblies
                .SelectMany(
                    assembly => assembly.GetTypes()
                        .Where(type => type is {IsPublic: true, IsClass: true, IsAbstract: false, IsGenericType: false})
                        .Where(type => type.GetMessageType(requiredBaseType) != null)
                        .Where(type => type.GetCustomAttribute<KafkaProducerAttribute>(false) != null)
                )
                .ToArray();
        }

        private static IServiceCollection AddMessageHandles(
            this IServiceCollection services,
            IEnumerable<Assembly> assemblies
        )
        {
            var messageHandlerTypes = assemblies
                .SelectMany(assembly => assembly.GetTypesAssignableFrom<IMessageHandler>())
                .ToArray();
            foreach (var messageHandlerType in messageHandlerTypes)
                services.AddSingleton(messageHandlerType);

            return services;
        }

        private static IServiceCollection AddProducers(
            this IServiceCollection services,
            IEnumerable<Type> producerTypes
        )
        {
            var producersMarkerInterfaceType = typeof(IKafkaProducer);
            var producerInterfaceType = typeof(IKafkaProducer<>);
            var producerBaseType = typeof(KafkaProducerBase<>);
            foreach (var producerType in producerTypes)
                services
                    .AddSingleton(producerType)
                    .AddSingleton(producersMarkerInterfaceType, producerType)
                    .AddSingleton(producerInterfaceType.MakeGenericType(producerType.GetMessageType(producerBaseType)), producerType);

            return services;
        }

        private static IServiceCollection AddConsumerServices(
            this IServiceCollection services,
            IEnumerable<Assembly> assemblies
        )
        {
            var baseType = typeof(IKafkaConsumer);
            var consumerTypes = assemblies
                .SelectMany(assembly => assembly.GetTypesAssignableFrom<IKafkaConsumer>())
                .ToArray();
            foreach (var consumerType in consumerTypes)
            {
                services.AddSingleton(consumerType);
                services.AddSingleton(baseType, consumerType);
            }

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
            var callingAssembly = Assembly.GetCallingAssembly();
            var assemblies = callingAssembly.LoadReferencedAssemblies().ToArray();
            var producerTypes = GetProducerTypes(assemblies);

            services
                .AddOptions()
                .Configure<KafkaSettings>(configuration.GetSection(nameof(KafkaSettings)));
            
            services
                .AddKafka(kafka => kafka.UseLogHandler<LoggerHandler>())
                .AddMessageHandles(assemblies)
                .AddConsumerServices(assemblies);

            var provider = services.BuildServiceProvider();
            var callingAssemblyName = callingAssembly.GetName().Name!;
            var kafkaSettings = configuration.GetSection(nameof(KafkaSettings)).Get<KafkaSettings>();
            return services
                .AddProducers(producerTypes)
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
                                        .AddConsumers(callingAssemblyName, provider.GetServices<IKafkaConsumer>());
                                }
                        )
                );
        }
    }
}