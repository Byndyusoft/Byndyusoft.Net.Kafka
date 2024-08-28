namespace MusicalityLabs.Storage.Api.MessageProducers
{
    using Byndyusoft.Net.Kafka.Producing;
    using KafkaFlow.Producers;
    using MusicalityLabs.Storage.Api.Contracts;

    [KafkaProducer("project.entity.creation")]
    public class EntityCreationEventMessageProducer : KafkaProducerBase<EntityCreation>
    {
        public EntityCreationEventMessageProducer(IProducerAccessor producers) : base(producers)
        {
        }

        protected override string KeyGenerator(EntityCreation entityCreation)
            => entityCreation.Id.ToString();
    }
}