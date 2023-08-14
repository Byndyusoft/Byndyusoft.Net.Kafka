﻿namespace Byndyusoft.Net.Kafka.Sample.Consumers;

using KafkaFlow.TypedHandler;
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