namespace MusicalityLabs.Storage.Api.Consumers.MessageHandlers
{
    using System;
    using System.Threading.Tasks;
    using KafkaFlow;
    using KafkaFlow.TypedHandler;
    using Microsoft.Extensions.Logging;
    using Contracts;

    public sealed class EntityCreationMessageHandler : IMessageHandler<EntityCreation>
    {
        private readonly ILogger<EntityCreationMessageHandler> _logger;

        public EntityCreationMessageHandler(ILogger<EntityCreationMessageHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task Handle(IMessageContext context, EntityCreation someEvent)
        {
            _logger.LogInformation(
                "Partition: {0} | Offset: {1} | Message: {2}",
                context.ConsumerContext.Partition,
                context.ConsumerContext.Offset,
                someEvent.Text
            );

            return Task.FromResult(someEvent);
        }
    }
}