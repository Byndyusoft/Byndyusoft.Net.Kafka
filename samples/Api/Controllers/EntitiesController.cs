namespace MusicalityLabs.ComposerAssistant.Storage.Api.Controllers;

using System;
using System.Threading;
using System.Threading.Tasks;
using Byndyusoft.Net.Kafka.Abstractions.Producing;
using Contracts;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class EntitiesController : ControllerBase, IEntitiesApi
{
    private readonly IKafkaMessageProducer<EntityCreation> _exampleProducer;

    public EntitiesController(IKafkaMessageProducer<EntityCreation> exampleProducer)
    {
        _exampleProducer = exampleProducer ?? throw new ArgumentNullException(nameof(exampleProducer));
    }

    /// <inheritdoc />
    [HttpPost]
    public async Task CreateEntity([FromBody] string text, CancellationToken cancellationToken)
    {
        var exampleMessageDto = new EntityCreation
                                {
                                    Text = text,
                                    Id = Guid.NewGuid()
                                };

        await _exampleProducer.ProduceAsync(exampleMessageDto);
    }
}