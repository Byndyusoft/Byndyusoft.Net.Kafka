using KafkaFlow;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace Byndyusoft.Net.Kafka.Extensions
{
    public static class HostApplicationLifetimeExtensions
    {
        /// <summary>
        ///     Start kafka bus
        /// </summary>
        public static void UseKafkaBus(this IHostApplicationLifetime lifetime, IApplicationBuilder app)
        {
            var kafkaBus = app.ApplicationServices.CreateKafkaBus();
            lifetime.ApplicationStarted.Register(() => kafkaBus.StartAsync(lifetime.ApplicationStopped));
        }
    }
}