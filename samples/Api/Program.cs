namespace MusicalityLabs.ComposerAssistant.Storage.Api
{
    using Byndyusoft.Logging.Builders;
    using Byndyusoft.Logging.Configuration;
    using Byndyusoft.Net.Kafka.Configuration;
    using Infrastructure.OpenTelemetry;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Hosting;
    using Serilog;

    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args)
                .Build()
                .StartKafkaProcessing()
                .Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            var serviceName = typeof(Program).Assembly.GetName().Name!;
            return Host.CreateDefaultBuilder(args)
                .UseSerilog(
                    (context, configuration) => configuration
                        .UseDefaultSettings(context.Configuration)
                        .UseOpenTelemetryTraces()
                        .WriteToOpenTelemetry(activityEventBuilder: StructuredActivityEventBuilder.Instance)
                )
                .ConfigureServices((context, services) => services.AddOpenTelemetry(serviceName, context.Configuration))
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
        }
    }
}