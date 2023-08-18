namespace Byndyusoft.Net.Kafka.Tests.Common.Generators;

using KafkaFlow;
using Moq;

public static class ProducerContextMockGenerator
{
    public static Mock<IProducerContext> TestProducerContextMock()
        => new Mock<IProducerContext>()
            .WithOffset(1)
            .WithPartition(2)
            .WithTopic("test_topic");

    public static Mock<IProducerContext> WithOffset(this Mock<IProducerContext> producerContextMock, int offset)
    {
        producerContextMock.Setup(x => x.Offset).Returns(offset);
        return producerContextMock;
    }

    public static Mock<IProducerContext> WithPartition(this Mock<IProducerContext> producerContextMock, int partition)
    {
        producerContextMock.Setup(x => x.Partition).Returns(partition);
        return producerContextMock;
    }

    public static Mock<IProducerContext> WithTopic(this Mock<IProducerContext> producerContextMock, string topic)
    {
        producerContextMock.Setup(x => x.Topic).Returns(topic);
        return producerContextMock;
    }
}