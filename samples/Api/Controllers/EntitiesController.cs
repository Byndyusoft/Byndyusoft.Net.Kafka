namespace MusicalityLabs.Storage.Api.Controllers
{
    using System;
    using System.Threading.Tasks;
    using Byndyusoft.Net.Kafka.Abstractions.Producing;
    using Microsoft.AspNetCore.Mvc;
    using Contracts;

    [ApiController]
    [Route("[controller]")]
    public class EntitiesController : ControllerBase
    {
        private readonly IKafkaMessageProducer<EntityCreation> _exampleProducer;

        public EntitiesController(IKafkaMessageProducer<EntityCreation> exampleProducer)
        {
            _exampleProducer = exampleProducer ?? throw new ArgumentNullException(nameof(exampleProducer));
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