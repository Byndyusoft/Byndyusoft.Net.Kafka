using System;
using System.Linq;
using System.Reflection;
using Byndyusoft.Net.Kafka.Abstractions;
using Byndyusoft.Net.Kafka.Handlers;
using KafkaFlow;
using KafkaFlow.TypedHandler;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Byndyusoft.Net.Kafka.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        ///     Register Kafka in DI
        /// </summary>
        public static IServiceCollection AddKafkaBus(
            this IServiceCollection services,
            IConfiguration configuration,
            Func<AssemblyName, bool> assemblyNamePredicate)
        {
            var callingAssembly = Assembly.GetCallingAssembly();
            return services
                .AddKafkaOptions(configuration)
                .AddKafkaServices(callingAssembly, assemblyNamePredicate)
                .AddKafka(configuration);
        }

        private static IServiceCollection AddKafkaOptions(
            this IServiceCollection services, 
            IConfiguration configuration)
        {
            return services
                .AddOptions()
                .Configure<KafkaSettings>(configuration.GetSection(nameof(KafkaSettings)))
                .Configure<KafkaSecurityInformationSettings>(
                    configuration.GetSection(nameof(KafkaSecurityInformationSettings)));
        }

        private static IServiceCollection AddKafkaServices(
            this IServiceCollection services,
            Assembly callingAssembly, 
            Func<AssemblyName, bool> assemblyNamePredicate)
        {
            var assemblies = callingAssembly.LoadReferencedAssemblies(assemblyNamePredicate).ToArray();

            return services
                .AddKafka(kafka => kafka.UseLogHandler<LoggerHandler>())
                .AddProducerServices(assemblies)
                .AddMessageHandles(assemblies)
                .AddConsumerServices(assemblies);
        }

        private static IServiceCollection AddKafka(
            this IServiceCollection services, 
            IConfiguration configuration)
        {
            var provider = services.BuildServiceProvider();

            var kafkaSettings = configuration.GetSection(nameof(KafkaSettings)).Get<KafkaSettings>();
            var kafkaSecurityInformationSettings = configuration.GetSection(nameof(KafkaSecurityInformationSettings))
                .Get<KafkaSecurityInformationSettings>();
            return services.AddKafka(
                kafka => kafka
                    .UseLogHandler<LoggerHandler>()
                    .AddCluster(
                        cluster =>
                        {
                            if (kafkaSettings.SecurityInformationEnabled)
                            {
                                cluster.WithSecurityInformation(information =>
                                {
                                    information.SaslUsername = kafkaSecurityInformationSettings.Username;
                                    information.SaslPassword = kafkaSecurityInformationSettings.Password;
                                    information.SaslMechanism = kafkaSecurityInformationSettings.SaslMechanism;
                                    information.SecurityProtocol = kafkaSecurityInformationSettings.SecurityProtocol;
                                });
                            }

                            cluster
                                .WithBrokers(kafkaSettings.Hosts)
                                .AddProducers(provider.GetServices<IKafkaProducer>(), kafkaSettings.Prefix)
                                .AddConsumers(provider.GetServices<IKafkaConsumer>(), kafkaSettings.Prefix);
                        })
            );
        }
        
        private static IServiceCollection AddProducerServices(
            this IServiceCollection services, 
            Assembly[] assemblies)
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
            Assembly[] assemblies)
        {
            var messageHandlerTypes = assemblies
                .SelectMany(assembly => assembly.GetTypesAssignableFrom<IMessageHandler>())
                .ToArray();
            foreach (var messageHandlerType in messageHandlerTypes)
                services.AddSingleton(messageHandlerType);

            return services;
        }

        // ReSharper disable once UnusedMethodReturnValue.Local
        private static IServiceCollection AddConsumerServices(
            this IServiceCollection services, 
            Assembly[] assemblies)
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