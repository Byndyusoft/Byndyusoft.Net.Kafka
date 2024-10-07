namespace Byndyusoft.Net.Kafka.Configuration
{
    using KafkaFlow;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    public static class HostExtensions
    {
        /// <summary>
        ///     Start kafka bus
        /// </summary>
        public static IHost StartKafkaProcessing(this IHost host)
        {
            var kafkaBus = host.Services.CreateKafkaBus();
            var lifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();
            lifetime.ApplicationStarted.Register(() => kafkaBus.StartAsync(lifetime.ApplicationStopped));

            return host;
        }
    }
}