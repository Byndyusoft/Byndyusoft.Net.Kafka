namespace Byndyusoft.Net.Kafka.Tests.Consuming;

using System.Threading.Tasks;
using Abstractions.Consuming;
using Kafka.Consuming;

[KafkaMessageHandler("composer_assistant.entity.creation")]
public class EntityCreationMessageHandler : KafkaMessageHandlerBase<EntityCreationMessage>
{
    protected override Task Handle(EntityCreationMessage message)
        => Task.CompletedTask;
}