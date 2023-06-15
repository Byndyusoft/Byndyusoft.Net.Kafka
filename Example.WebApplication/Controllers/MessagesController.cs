using System;
using System.Threading.Tasks;
using Byndyusoft.Example.Domain.Dtos;
using Byndyusoft.Example.Domain.Producers;
using Microsoft.AspNetCore.Mvc;

namespace Byndyusoft.Example.WebApplication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly ExampleProducer _exampleProducer;

        public MessagesController(ExampleProducer exampleProducer)
        {
            _exampleProducer = exampleProducer;
        }

        [HttpPost]
        public async Task Post([FromBody] string text)
        {
            var exampleMessageDto = new ExampleMessageDto
            {
                Text = text,
                Guid = Guid.NewGuid()
            };

            await _exampleProducer.ProduceAsync(exampleMessageDto);
        }
    }
}