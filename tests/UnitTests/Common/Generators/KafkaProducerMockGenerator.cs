using Moq;

namespace Byndyusoft.Net.Kafka.Tests.Common.Generators;

public static class KafkaProducerMockGenerator
{
    public static Mock<IKafkaProducer> Empty() => new();

    public static Mock<IKafkaProducer> WithTopic(
        this Mock<IKafkaProducer> kafkaProducerMock,
        string topic
    )
    {
        kafkaProducerMock.Setup(x => x.Topic).Returns(topic);
        return kafkaProducerMock;
    }
}