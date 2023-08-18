using KafkaFlow;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace Byndyusoft.Net.Kafka.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        ///     Start kafka bus
        /// </summary>
        public static void UseKafkaBus(this IApplicationBuilder app, IHostApplicationLifetime lifetime)
        {
            var kafkaBus = app.ApplicationServices.CreateKafkaBus();
            lifetime.ApplicationStarted.Register(() => kafkaBus.StartAsync(lifetime.ApplicationStopped));
        }
    }
}