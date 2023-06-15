using System.Threading.Tasks;
using Byndyusoft.Example.Domain.Dtos;
using Byndyusoft.Example.Domain.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Byndyusoft.Example.Domain.Services
{
    public sealed class ExampleService : IExampleService
    {
        private readonly ILogger<ExampleService> _logger;

        public ExampleService(ILogger<ExampleService> logger)
        {
            _logger = logger;
        }

        public Task DoSomething(ExampleMessageDto message)
        {
            _logger.LogInformation("Message with Guid: {Guid} and text {Text} was arrived", message.Guid, message.Text);
        
            //Do some work
            return Task.CompletedTask; 
        }
    }
}