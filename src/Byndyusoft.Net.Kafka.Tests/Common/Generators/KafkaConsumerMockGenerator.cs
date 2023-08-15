namespace Byndyusoft.Net.Kafka.Tests.Common.Generators;

using Moq;

public static class KafkaConsumerMockGenerator
{
    public static Mock<IKafkaConsumer> Empty() => new();

    public static Mock<IKafkaConsumer> WithTopic(
        this Mock<IKafkaConsumer> kafkaConsumerMock,
        string topic
    )
    {
        kafkaConsumerMock.Setup(x => x.Topic).Returns(topic);
        return kafkaConsumerMock;
    }
}