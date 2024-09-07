namespace MusicalityLabs.ComposerAssistant.Storage.Api.MessageHandlers
{
    using System;
    using System.Threading.Tasks;
    using Byndyusoft.Net.Kafka.Abstractions.Consuming;
    using Byndyusoft.Net.Kafka.Consuming;
    using Microsoft.Extensions.Logging;
    using Contracts;

    [KafkaMessageHandler(topic: "composer_assistant.entity.creation")]
    public sealed class EntityCreationMessageHandler : KafkaMessageHandlerBase<EntityCreation>
    {
        private readonly ILogger<EntityCreationMessageHandler> _logger;

        public EntityCreationMessageHandler(ILogger<EntityCreationMessageHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override Task Handle(EntityCreation someEvent)
        {
            _logger.LogInformation("Message: {EntityText}", someEvent.Text);
            return Task.FromResult(someEvent);
        }
    }
}