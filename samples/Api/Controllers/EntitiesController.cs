namespace MusicalityLabs.Storage.Api.Controllers
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Contracts;
    using MessageProducers;

    [ApiController]
    [Route("[controller]")]
    public class EntitiesController : ControllerBase
    {
        private readonly EntityCreationEventMessageProducer _exampleProducer;

        public EntitiesController(EntityCreationEventMessageProducer exampleProducer)
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
}