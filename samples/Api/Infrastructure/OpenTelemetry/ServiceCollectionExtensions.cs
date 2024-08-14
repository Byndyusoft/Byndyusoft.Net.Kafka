namespace MusicalityLabs.Storage.Api.Infrastructure.OpenTelemetry
{
    using global::OpenTelemetry.Metrics;
    using global::OpenTelemetry.Resources;
    using global::OpenTelemetry.Trace;
    using KafkaFlow.OpenTelemetry;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddOpenTelemetry(
            this IServiceCollection services,
            string serviceName,
            IConfiguration configuration
        ) => services
            .AddOpenTelemetryTracing(
                builder => builder
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName))
                    .AddSource(KafkaFlowInstrumentation.ActivitySourceName)
                    .AddAspNetCoreInstrumentation()
                    .AddJaegerExporter(
                        o =>
                            {
                                o.AgentHost = configuration.GetSection("Jaeger:JAEGER_AGENT_HOST").Value;
                                o.AgentPort = int.Parse(configuration.GetSection("Jaeger:JAEGER_AGENT_PORT").Value);
                            }
                        )
            )
            .AddOpenTelemetryMetrics(
                builder => builder
                    .AddAspNetCoreInstrumentation()
                    .AddPrometheusExporter()
            );
    }
}