namespace Byndyusoft.Net.Kafka.Tests.Common.Generators;

using KafkaFlow;
using Moq;

public static class ConsumerContextMockGenerator
{
    public static Mock<IConsumerContext> TestConsumerContextMock()
        => new Mock<IConsumerContext>()
            .WithOffset(1)
            .WithPartition(2)
            .WithTopic("test_topic");

    public static Mock<IConsumerContext> WithOffset(this Mock<IConsumerContext> consumerContextMock, int offset)
    {
        consumerContextMock.Setup(x => x.Offset).Returns(offset);
        return consumerContextMock;
    }

    public static Mock<IConsumerContext> WithPartition(this Mock<IConsumerContext> consumerContextMock, int partition)
    {
        consumerContextMock.Setup(x => x.Partition).Returns(partition);
        return consumerContextMock;
    }

    public static Mock<IConsumerContext> WithTopic(this Mock<IConsumerContext> consumerContextMock, string topic)
    {
        consumerContextMock.Setup(x => x.Topic).Returns(topic);
        return consumerContextMock;
    }
}