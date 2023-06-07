using System;
using System.Threading.Tasks;
using Byndyusoft.Example.WebApplication.Producers;
using Microsoft.AspNetCore.Mvc;

namespace Byndyusoft.Example.WebApplication.Controllers;

[ApiController]
[Route("[controller]")]
public class MessageController : ControllerBase
{   
    private readonly ExampleProducer _exampleProducer;

    public MessageController(ExampleProducer exampleProducer)
    {
        _exampleProducer = exampleProducer;
    }

    [HttpPost]
    public async Task Post(ExampleMessageDto exampleMessageDto)
    {
        await _exampleProducer.ProduceAsync(exampleMessageDto);
    }
}