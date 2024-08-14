namespace Byndyusoft.Net.Kafka.Extensions
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using DependencyInjection;
    using Handlers;
    using KafkaFlow;
    using KafkaFlow.Configuration;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;

    public static class ServiceCollectionExtensions
    {
        private static IServiceCollection AddKafkaOptions(
            this IServiceCollection services,
            IConfiguration configuration
        ) => services
            .AddOptions()
            .Configure<KafkaSettings>(configuration.GetSection(nameof(KafkaSettings)));

        private static IServiceCollection AddKafka(
            this IServiceCollection services,
            Assembly callingAssembly
        )
        {
            var callingAssemblyName = callingAssembly.GetName().Name!;
            var dependencyConfigurator = new DependencyConfigurator(services);
            return services
                .AddSingleton<IDependencyResolver>(provider => new DependencyResolver(provider))
                .AddSingleton(
                    provider =>
                        {
                            var kafkaSettings = provider.GetRequiredService<IOptions<KafkaSettings>>().Value;
                            return new KafkaFlowConfigurator(
                                dependencyConfigurator,
                                kafka => kafka
                                    .UseLogHandler<LoggerHandler>()
                                    .AddOpenTelemetryInstrumentation()
                                    .AddCluster(
                                        cluster => cluster
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
                                            .AddProducers(callingAssemblyName, provider.GetServices<IKafkaProducer>())
                                            .AddConsumers(callingAssemblyName, provider.GetServices<IKafkaConsumer>())
                                    )
                            );
                        }
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
            return services
                .AddKafkaOptions(configuration)
                .AddKafka(callingAssembly)
                .AddProducerServices(assemblies)
                .AddMessageHandles(assemblies)
                .AddConsumerServices(assemblies);
        }
    }
}