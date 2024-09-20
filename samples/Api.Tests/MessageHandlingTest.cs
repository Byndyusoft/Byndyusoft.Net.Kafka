namespace MusicalityLabs.ComposerAssistant.Storage.Api.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Options;
    using Clients;
    using Infrastructure.Logging;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Xunit;
    using Moq;

    public class MessageHandlingTest
    {
        private static Task<IHost> CreateHost(Mock<ILogger> mockLogger)
            => new HostBuilder()
                .UseMockLogger(mockLogger)
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
                )
                .ConfigureWebHost(
                    webBuilder => webBuilder
                        .UseTestServer()
                        .UseStartup<Startup>()
                )
                .StartAsync();

        private static EntitiesApiClient CreateEntitiesApiClient(HttpClient httpClient)
            => new(
                httpClient,
                Options.Create(new StorageApiSettings {ConnectionString = httpClient.BaseAddress!.ToString()})
            );

        [Fact]
        public async Task ShouldHandleMessageFromKafka()
        {
            // Given
            const string entityCreatingText = "Api.Testing.Test";

            var mockLogger = MockLoggerExtensions.CreateMockLogger();
            using var host = await CreateHost(mockLogger);
            
            // When
            var entitiesApiClient = CreateEntitiesApiClient(host.GetTestClient());
            await entitiesApiClient.CreateEntity(entityCreatingText, CancellationToken.None);

            // Then
            await Task.Delay(TimeSpan.FromSeconds(30));
            mockLogger
                .VerifyNoErrorsWasLogged()
                .VerifyInformationMessageWasLogged($"Message: {entityCreatingText}");
        }
    }
}