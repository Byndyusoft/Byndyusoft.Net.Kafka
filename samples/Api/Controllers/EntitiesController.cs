namespace Api.Controllers;

using System;
using System.Threading.Tasks;
using Byndyusoft.Net.Kafka.Api.Contracts;
using Microsoft.AspNetCore.Mvc;
using Producers;

[ApiController]
[Route("[controller]")]
public class EntitiesController : ControllerBase
{
    private readonly EntityCreationEventsProducer _exampleProducer;

    public EntitiesController(EntityCreationEventsProducer exampleProducer)
    {
        _exampleProducer = exampleProducer;
    }

    [HttpPost]
    public async Task CreateEntity([FromBody] string text)
    {
        var exampleMessageDto = new EntityCreation
                                {
                                    Text = text,
                                    Id = Guid.NewGuid()
                                };

        await _exampleProducer.ProduceAsync(exampleMessageDto);
    }
}