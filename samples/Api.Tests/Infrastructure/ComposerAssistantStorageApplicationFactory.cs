namespace MusicalityLabs.ComposerAssistant.Storage.Api.Tests.Infrastructure;

using System.Collections.Generic;
using Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

public class ComposerAssistantStorageApplicationFactory : WebApplicationFactory<Program>
{
    private readonly Mock<ILogger> _mockLogger;

    public ComposerAssistantStorageApplicationFactory(Mock<ILogger> mockLogger)
    {
        _mockLogger = mockLogger;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
        => builder
            .UseMockLogger(_mockLogger)
            .ConfigureAppConfiguration(
                (_, configBuilder) => configBuilder
                    .AddInMemoryCollection(
                        new[]
                        {
                            new KeyValuePair<string, string>("KafkaSettings:Hosts:0", "127.0.0.1:9092"),
                            new KeyValuePair<string, string>("KafkaSettings:Username", "broker"),
                            new KeyValuePair<string, string>("KafkaSettings:Password", "broker")
                        }
                    )
            );
}