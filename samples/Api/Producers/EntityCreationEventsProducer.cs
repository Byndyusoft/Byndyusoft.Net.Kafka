namespace MusicalityLabs.Storage.Api.Producers
{
    using KafkaFlow.Producers;
    using Contracts;
    using Byndyusoft.Net.Kafka.Producing;

    [KafkaProducer("project.entity.creation")]
    public class EntityCreationEventsProducer : KafkaProducerBase<EntityCreation>
    {
        public EntityCreationEventsProducer(IProducerAccessor producers) : base(producers)
        {
        }

        protected override string KeyGenerator(EntityCreation entityCreation)
            => entityCreation.Id.ToString();
    }
}