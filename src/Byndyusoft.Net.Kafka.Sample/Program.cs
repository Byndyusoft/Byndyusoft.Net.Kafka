namespace Byndyusoft.Net.Kafka.Sample;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Tracing;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
        => Host
            .CreateDefaultBuilder(args)
            .ConfigureServices(
                (context, services) =>
                    services.AddOpenTracingServices(
                            options => options
                                .AddDefaultIgnorePatterns()
                                .WithDefaultOperationNameResolver()
                        )
                        .AddJaegerTracer(context.Configuration)
            )
            .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
}