using System.Threading.Tasks;
using Byndyusoft.Example.Domain.Dtos;
using Byndyusoft.Example.Domain.Services.Interfaces;
using KafkaFlow;
using KafkaFlow.TypedHandler;

namespace Byndyusoft.Example.Domain.MessageHandlers
{
    public sealed class ExampleMessageHandler : IMessageHandler<ExampleMessageDto>
    {
        private readonly IExampleService _exampleService;

        public ExampleMessageHandler(IExampleService exampleService)
        {
            _exampleService = exampleService;
        }

        public async Task Handle(IMessageContext context, ExampleMessageDto message)
        {
            await _exampleService.DoSomething(message);
        }
    }
}