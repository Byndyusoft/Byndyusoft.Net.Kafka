namespace MusicalityLabs.ComposerAssistant.Storage.Api.MessageProducers;

using Byndyusoft.Net.Kafka.Abstractions.Producing;
using Byndyusoft.Net.Kafka.Producing;
using Contracts;

[KafkaMessageProducer("composer_assistant.entity.creation")]
public class EntityCreationEventMessageProducer : KafkaMessageProducerBase<EntityCreation>
{
    public EntityCreationEventMessageProducer(IKafkaMessageSender messageSender) : base(messageSender)
    {
    }

    protected override string KeyGenerator(EntityCreation entityCreation)
        => entityCreation.Id.ToString();
}