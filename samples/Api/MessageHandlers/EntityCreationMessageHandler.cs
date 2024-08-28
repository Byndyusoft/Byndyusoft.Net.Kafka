namespace MusicalityLabs.Storage.Api.MessageHandlers
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Contracts;
    using Byndyusoft.Net.Kafka.Consuming;

    [KafkaMessageHandler("project.entity.creation")]
    public sealed class EntityCreationMessageHandler : KafkaMessageHandlerBase<EntityCreation>
    {
        private readonly ILogger<EntityCreationMessageHandler> _logger;

        public EntityCreationMessageHandler(ILogger<EntityCreationMessageHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        protected override Task Handle(EntityCreation someEvent)
        {
            _logger.LogInformation("Message: {2}", someEvent.Text);
            return Task.FromResult(someEvent);
        }
    }
}