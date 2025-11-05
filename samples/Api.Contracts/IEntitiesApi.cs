namespace MusicalityLabs.ComposerAssistant.Storage.Api.Contracts;

using System.Threading;
using System.Threading.Tasks;

public interface IEntitiesApi
{
    /// <summary>
    ///     Creates entity with text <paramref name="text" />
    /// </summary>
    Task CreateEntity(string text, CancellationToken cancellationToken);
}