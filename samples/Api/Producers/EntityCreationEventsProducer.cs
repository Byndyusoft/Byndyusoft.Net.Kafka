namespace Api.Producers
{
    using Byndyusoft.Net.Kafka;
    using Byndyusoft.Net.Kafka.Api.Contracts;
    using KafkaFlow.Producers;

    public class EntityCreationEventsProducer : KafkaProducerBase<EntityCreation>
    {
        public EntityCreationEventsProducer(IProducerAccessor producers) 
            : base(producers, nameof(EntityCreationEventsProducer))
        {
        }

        public override string Topic => "project.entity.creation";
        
        public override string KeyGenerator(EntityCreation entityCreation)
            => entityCreation.Id.ToString();
    }
}