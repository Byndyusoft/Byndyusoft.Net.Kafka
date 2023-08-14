namespace Byndyusoft.Net.Kafka.Sample.Consumers.MessageHandlers;

using System;
using System.Threading.Tasks;
using Contracts;
using KafkaFlow;
using KafkaFlow.TypedHandler;

public sealed class EntityCreationMessageHandler : IMessageHandler<EntityCreation>
{
    public Task Handle(IMessageContext context, EntityCreation someEvent)
    {
        Console.WriteLine(
            "Partition: {0} | Offset: {1} | Message: {2}",
            context.ConsumerContext.Partition,
            context.ConsumerContext.Offset,
            someEvent.Text
        );

        return Task.FromResult(someEvent);
    }
}