namespace MusicalityLabs.Storage.Api.Producers
{
    using KafkaFlow.Producers;
    using Contracts;
    using Byndyusoft.Net.Kafka.Producing;

    public class EntityCreationEventsProducer : KafkaProducerBase<EntityCreation>
    {
        public EntityCreationEventsProducer(IProducerAccessor producers)
            : base(producers, nameof(EntityCreationEventsProducer))
        {
        }

        public override string Topic => "project.entity.creation";

        public override string KeyGenerator(EntityCreation entityCreation)
        {
            return entityCreation.Id.ToString();
        }
    }
}