namespace Byndyusoft.Net.Kafka.Tests.Producing
{
    using System.Globalization;
    using Abstractions.Producing;
    using Kafka.Producing;

    [KafkaMessageProducer(topic: "composer_assistant.entity.creation", ProducingProfileName = nameof(EntityCreationEventMessageCustomProducer))]
    public class EntityCreationEventMessageCustomProducer : KafkaMessageProducerBase<EntityCreationMessage>
    {
        public EntityCreationEventMessageCustomProducer(IKafkaMessageSender messageSender) : base(messageSender)
        {
        }

        protected override string KeyGenerator(EntityCreationMessage message)
            => message.Id.ToString(CultureInfo.InvariantCulture);
    }
}