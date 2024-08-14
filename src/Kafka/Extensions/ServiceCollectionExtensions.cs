namespace Byndyusoft.Net.Kafka.Extensions
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Handlers;
    using KafkaFlow;
    using KafkaFlow.TypedHandler;
    using Mapster;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceCollectionExtensions
    {
        /// <summary>
        ///     Register Kafka in DI
        /// </summary>
        public static IServiceCollection AddKafkaBus(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            var callingAssembly = Assembly.GetCallingAssembly();
            var assemblies = callingAssembly.LoadReferencedAssemblies().ToArray();

            return services
                .AddKafkaOptions(configuration)
                .AddKafka(configuration, callingAssembly)
                .AddProducerServices(assemblies)
                .AddMessageHandles(assemblies)
                .AddConsumerServices(assemblies);
        }

        private static IServiceCollection AddKafkaOptions(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            return services
                .AddOptions()
                .Configure<KafkaSettings>(configuration.GetSection(nameof(KafkaSettings)));
        }

        private static IServiceCollection AddKafka(
            this IServiceCollection services,
            IConfiguration configuration,
            Assembly callingAssembly
        )
        {
            var provider = services.BuildServiceProvider();

            var kafkaSettings = configuration.GetSection(nameof(KafkaSettings)).Get<KafkaSettings>();
            var callingAssemblyName = callingAssembly.GetName().Name!;

            return services.AddKafka(
                kafka => kafka
                    .UseLogHandler<LoggerHandler>()
                    .AddCluster(
                        cluster =>
                            {
                                cluster
                                    .WithBrokers(kafkaSettings.Hosts)
                                    .WithSecurityInformation(
                                        information =>
                                            {
                                                if (kafkaSettings.SecurityInformation != null)
                                                    information.Adapt(kafkaSettings.SecurityInformation);
                                            }
                                    )
                                    .AddProducers(provider.GetServices<IKafkaProducer>(), callingAssemblyName, kafkaSettings.ProducerSettings)
                                    .AddConsumers(provider.GetServices<IKafkaConsumer>(), callingAssemblyName);
                            }
                    )
            );
        }

        private static IServiceCollection AddProducerServices(
            this IServiceCollection services,
            IEnumerable<Assembly> assemblies
        )
        {
            var baseType = typeof(IKafkaProducer);
            var producerTypes = assemblies
                .SelectMany(assembly => assembly.GetTypesAssignableFrom<IKafkaProducer>())
                .ToArray();
            foreach (var producerType in producerTypes)
            {
                services.AddSingleton(producerType);
                services.AddSingleton(baseType, producerType);
            }

            return services;
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
    }
}