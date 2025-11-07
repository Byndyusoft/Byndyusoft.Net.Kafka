namespace MusicalityLabs.ComposerAssistant.Storage.Api.Clients;

using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Byndyusoft.ApiClient;
using Contracts;
using Microsoft.Extensions.Options;

public class EntitiesApiClient : BaseClient, IEntitiesApi
{
    private const string ApiPrefix = "api/entities";

    public EntitiesApiClient(HttpClient client, IOptions<StorageApiSettings> apiSettings) : base(client, apiSettings)
    {
    }

    public Task CreateEntity(string text, CancellationToken cancellationToken)
        => PostAsync(ApiPrefix, text, cancellationToken);
}