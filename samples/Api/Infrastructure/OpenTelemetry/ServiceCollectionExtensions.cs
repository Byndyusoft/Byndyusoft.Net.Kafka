namespace MusicalityLabs.ComposerAssistant.Storage.Api.Infrastructure.OpenTelemetry;

using System;
using Byndyusoft.Net.Kafka.Configuration;
using global::OpenTelemetry.Exporter;
using global::OpenTelemetry.Resources;
using global::OpenTelemetry.Trace;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOpenTelemetry(
        this IServiceCollection services,
        string serviceName,
        IConfiguration configuration
    )
    {
        services
            .AddOpenTelemetry()
            .ConfigureResource(
                resource => resource.AddService(serviceName)
            )
            .WithTracing(
                builder =>
                    builder
                        .AddKafkaInstrumentation()
                        .AddAspNetCoreInstrumentation(
                            o =>
                            {
                                o.Filter = context => !context.Request.Path
                                               .StartsWithSegments("/swagger");
                            }
                        )
                        .AddOtlpExporter(
                            o =>
                            {
                                o.Endpoint = new Uri(configuration.GetSection("Jaeger:JAEGER_ENDPOINT").Value!);
                                o.Protocol = OtlpExportProtocol.HttpProtobuf;
                            }
                        )
            );

        return services;
    }
}