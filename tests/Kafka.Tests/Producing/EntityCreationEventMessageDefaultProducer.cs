namespace Byndyusoft.Net.Kafka.Tests.Producing
{
    using System.Globalization;
    using Abstractions.Producing;
    using Kafka.Producing;

    [KafkaMessageProducer(topic: "composer_assistant.entity.creation")]
    public class EntityCreationEventMessageDefaultProducer : KafkaMessageProducerBase<EntityCreationMessage>
    {
        public EntityCreationEventMessageDefaultProducer(IKafkaMessageSender messageSender) : base(messageSender)
        {
        }

        protected override string KeyGenerator(EntityCreationMessage message)
            => message.Id.ToString(CultureInfo.InvariantCulture);
    }
}