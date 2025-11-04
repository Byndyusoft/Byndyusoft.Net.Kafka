namespace MusicalityLabs.ComposerAssistant.Storage.Api;

using Byndyusoft.Logging.Builders;
using Byndyusoft.Logging.Configuration;
using Byndyusoft.Net.Kafka;
using Byndyusoft.Net.Kafka.Configuration;
using Infrastructure.OpenTelemetry;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Configuration;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Host
            .UseSerilog(
                (context, configuration) => configuration
                    .UseDefaultSettings(context.Configuration)
                    .UseOpenTelemetryTraces()
                    .WriteToOpenTelemetry(activityEventBuilder: StructuredActivityEventBuilder.Instance)
                    .Enrich.WithPropertyDataAccessor()
                    .Enrich.WithStaticTelemetryItems()
            );

        var serviceName = typeof(Program).Assembly.GetName().Name!;
        builder.Services
            .AddOpenTelemetry(serviceName, builder.Configuration);

        builder.Services
            .AddControllers()
            .AddOpenTelemetryTracing();

        builder.Services
            .AddSwaggerGen()
            .AddKafkaBus(builder.Configuration.GetSection(nameof(KafkaSettings)).Get<KafkaSettings>()!);

        builder.Services
            .ConfigureStaticTelemetryItemCollector()
            .WithBuildConfiguration()
            .WithAspNetCoreEnvironment()
            .WithServiceName(serviceName);
        
        var app = builder.Build();

        if (app.Environment.IsDevelopment())
            app
                .UseSwagger()
                .UseSwaggerUI();
        app
            .UseHttpsRedirection()
            .UseRouting();

        app.MapControllers();

        app
            .StartKafkaProcessing()
            .Run();
    }
}