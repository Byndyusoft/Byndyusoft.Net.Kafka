namespace MusicalityLabs.ComposerAssistant.Storage.Api.Infrastructure.OpenTelemetry;

using System.Text.Json;
using System.Text.Json.Serialization;
using Byndyusoft.AspNetCore.Instrumentation.Tracing.Serialization.Json;
using Microsoft.Extensions.DependencyInjection;

public static class MpvBuilderExtensions
{
    public static IMvcBuilder AddOpenTelemetryTracing(this IMvcBuilder mvcBuilder)
        => mvcBuilder
            .AddTracing(
                o =>
                {
                    o.EnrichTraceWithTaggedRequestParams = true;
                    o.EnrichLogsWithParams = true;
                    o.EnrichLogsWithHttpInfo = true;
                    o.Formatter = new SystemTextJsonFormatter
                                  {
                                      Options = new JsonSerializerOptions
                                                {
                                                    Converters =
                                                    {
                                                        new JsonStringEnumConverter()
                                                    }
                                                }
                                  };
                }
            );
}