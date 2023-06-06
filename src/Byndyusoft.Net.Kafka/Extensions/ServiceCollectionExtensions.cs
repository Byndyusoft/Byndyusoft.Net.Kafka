using System.Linq;
using System.Reflection;
using Byndyusoft.Net.Kafka.Handlers;
using KafkaFlow;
using KafkaFlow.Configuration;
using KafkaFlow.TypedHandler;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Byndyusoft.Net.Kafka.Extensions
{
    public static class ServiceCollectionExtensions
    {
        private static IServiceCollection AddConsumerServices(this IServiceCollection services, Assembly[] assemblies)
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

        private static IServiceCollection AddProducerServices(this IServiceCollection services, Assembly[] assemblies)
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

        private static IServiceCollection AddMessageHandles(this IServiceCollection services, Assembly[] assemblies)
        {
            var messageHandlerTypes = assemblies
                .SelectMany(assembly => assembly.GetTypesAssignableFrom<IMessageHandler>())
                .ToArray();
            foreach (var messageHandlerType in messageHandlerTypes)
                services.AddSingleton(messageHandlerType);

            return services;
        }

        public static IServiceCollection AddKafkaBus(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions().Configure<KafkaSettings>(configuration.GetSection(nameof(KafkaSettings)));

            var callingAssembly = Assembly.GetCallingAssembly();
            var assemblies = callingAssembly.LoadReferencedAssemblies().ToArray();
            services
                .AddKafka(kafka => kafka.UseLogHandler<LoggerHandler>())
                .AddProducerServices(assemblies)
                .AddMessageHandles(assemblies)
                .AddConsumerServices(assemblies);

            var provider = services.BuildServiceProvider();

            var callingAssemblyName = callingAssembly.GetName().Name!;
            var kafkaSettings = configuration.GetSection(nameof(KafkaSettings)).Get<KafkaSettings>();
            services.AddKafka(
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
                                        information.SaslMechanism = SaslMechanism.ScramSha512;
                                        information.SecurityProtocol = SecurityProtocol.SaslPlaintext;
                                        information.SaslUsername = kafkaSettings.Username;
                                        information.SaslPassword = kafkaSettings.Password;
                                    })
                                .AddProducers(provider.GetServices<IKafkaProducer>(), callingAssemblyName)
                                .AddConsumers(provider.GetServices<IKafkaConsumer>(), callingAssemblyName);
                        })
            );

            return services;
        }
    }
}