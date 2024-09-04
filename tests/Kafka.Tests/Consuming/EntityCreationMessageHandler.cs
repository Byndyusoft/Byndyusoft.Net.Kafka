namespace Byndyusoft.Net.Kafka.Tests.Consuming
{
    using System.Threading.Tasks;
    using Byndyusoft.Net.Kafka.Abstractions.Consuming;
    using Kafka.Consuming;

    [KafkaMessageHandler(topic: "composer_assistant.entity.creation")]
    public class EntityCreationMessageHandler : KafkaMessageHandlerBase<EntityCreationMessage>
    {
        protected override Task Handle(EntityCreationMessage message)
        {
            return Task.CompletedTask;
        }
    }
}