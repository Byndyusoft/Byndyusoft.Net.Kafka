namespace Byndyusoft.Net.Kafka.Tests.Common.TestDatasets.Consumers.MessageHandlers;

using System.Threading.Tasks;
using KafkaFlow;
using KafkaFlow.TypedHandler;

public sealed class EntityCreationMessageHandler : IMessageHandler<EntityCreation>
{
    public Task Handle(IMessageContext context, EntityCreation someEvent)
        => Task.FromResult(someEvent);
}