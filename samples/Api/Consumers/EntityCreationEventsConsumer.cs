namespace MusicalityLabs.Storage.Api.Consumers
{
    using KafkaFlow;
    using Byndyusoft.Net.Kafka;
    using MessageHandlers;
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