namespace MusicalityLabs.Storage.Api.Consumers
{
    using KafkaFlow;
    using MessageHandlers;
    using Byndyusoft.Net.Kafka.Consuming;

    public sealed class EntityCreationEventsConsumer : IKafkaConsumer
    {
        public EntityCreationEventsConsumer(EntityCreationMessageHandler messageHandler)
        {
            MessageHandler = messageHandler;
        }

        public string Topic => "project.entity.creation";

        public IMessageHandler MessageHandler { get; }
    }
}