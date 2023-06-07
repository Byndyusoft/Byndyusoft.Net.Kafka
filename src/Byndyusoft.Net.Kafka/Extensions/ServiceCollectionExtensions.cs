using System.Linq;
using System.Reflection;
using Byndyusoft.Net.Kafka.Handlers;
using KafkaFlow;
using KafkaFlow.Configuration;
using KafkaFlow.Producers;
using KafkaFlow.TypedHandler;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Byndyusoft.Net.Kafka.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddKafkaBus(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddOptions()
                .Configure<KafkaSettings>(configuration.GetSection(nameof(KafkaSettings)));

            var provider = services.BuildServiceProvider();

            var kafkaSettings = configuration.GetSection(nameof(KafkaSettings)).Get<KafkaSettings>();
            services.AddKafka(
                kafka => kafka
                    .UseLogHandler<LoggerHandler>()
                    .AddCluster(
                        cluster =>
                        {
                            cluster
                                .WithBrokers(kafkaSettings.Hosts)
                                .AddProducers(provider.GetServices<IKafkaProducer>(), kafkaSettings.Prefix, kafkaSettings.ServiceName)
                                .AddConsumers(provider.GetServices<IKafkaConsumer>(), kafkaSettings.Prefix, kafkaSettings.ServiceName);
                        })
            );

            return services;
        }
    }
}
// TODO
/*
 * .WithSecurityInformation(
                                    information =>
                                    {
                                        information.SaslMechanism = SaslMechanism.ScramSha512;
                                        information.SecurityProtocol = SecurityProtocol.SaslPlaintext;
                                        information.SaslUsername = kafkaSettings.Username;
                                        information.SaslPassword = kafkaSettings.Password;
                                    })
 */