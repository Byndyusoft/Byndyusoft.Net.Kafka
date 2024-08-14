namespace MusicalityLabs.Storage.Api.Producers
{
    using Byndyusoft.Net.Kafka;
    using KafkaFlow.Producers;
    using Contracts;

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