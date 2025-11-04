namespace MusicalityLabs.ComposerAssistant.Storage.Api.Tests;

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Clients;
using Infrastructure;
using Infrastructure.Logging;
using Microsoft.Extensions.Options;
using Xunit;

public class MessageHandlingTest
{
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
        var applicationFactory = new ComposerAssistantStorageApplicationFactory(mockLogger);

        // When
        var entitiesApiClient = CreateEntitiesApiClient(applicationFactory.CreateClient());
        await entitiesApiClient.CreateEntity(entityCreatingText, CancellationToken.None);

        // Then
        await Task.Delay(TimeSpan.FromSeconds(30));
        mockLogger
            .VerifyNoErrorsWasLogged()
            .VerifyInformationMessageWasLogged($"Message: {entityCreatingText}");
    }
}