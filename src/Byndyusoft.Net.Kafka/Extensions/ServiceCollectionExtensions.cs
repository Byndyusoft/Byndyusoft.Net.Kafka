using System;
using Byndyusoft.Net.Kafka.Handlers;
using KafkaFlow;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Byndyusoft.Net.Kafka.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        ///     Register and start Kafka
        /// </summary>
        public static IServiceCollection AddKafkaBus(this IServiceCollection services,
            IConfiguration configuration,
            Action<IServiceCollection> registerProducersAndConsumersAndMessageHandlers)
        {
            services
                .AddOptions()
                .Configure<KafkaSettings>(configuration.GetSection(nameof(KafkaSettings)))
                .Configure<KafkaSecurityInformationSettings>(configuration.GetSection(nameof(KafkaSecurityInformationSettings)));

            services.AddKafka(kafka => kafka.UseLogHandler<LoggerHandler>());
            registerProducersAndConsumersAndMessageHandlers(services);

            var provider = services.BuildServiceProvider();

            var kafkaSettings = configuration.GetSection(nameof(KafkaSettings)).Get<KafkaSettings>();
            var kafkaSecurityInformationSettings = configuration.GetSection(nameof(KafkaSecurityInformationSettings)).Get<KafkaSecurityInformationSettings>();
            services.AddKafka(
                kafka => kafka
                    .UseLogHandler<LoggerHandler>()
                    .AddCluster(
                        cluster =>
                        {
                            if (kafkaSettings.SecurityInformationEnabled)
                            {
                                cluster.WithSecurityInformation(information =>
                                    {
                                        information.SaslMechanism = kafkaSecurityInformationSettings.SaslMechanism;
                                        information.SecurityProtocol = kafkaSecurityInformationSettings.SecurityProtocol;
                                        information.SaslUsername = kafkaSecurityInformationSettings.Username;
                                        information.SaslPassword = kafkaSecurityInformationSettings.Password;
                                    });
                            }
                            cluster
                                .WithBrokers(kafkaSettings.Hosts)
                                .AddProducers(provider.GetServices<IKafkaProducer>(), kafkaSettings.Prefix)
                                .AddConsumers(provider.GetServices<IKafkaConsumer>(), kafkaSettings.Prefix);
                        })
            );

            return services;
        }
    }
}