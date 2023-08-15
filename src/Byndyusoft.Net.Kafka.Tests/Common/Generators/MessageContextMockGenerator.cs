namespace Byndyusoft.Net.Kafka.Tests.Common.Generators;

using KafkaFlow;
using Moq;

public static class MessageContextMockGenerator
{
    public static Mock<IMessageContext> WithMessage(
        this Mock<IMessageContext> messageContextMock,
        object messageKey,
        object messageValue
    )
    {
        messageContextMock.Setup(x => x.Message).Returns(new Message(messageKey, messageValue));
        return messageContextMock;
    }

    public static Mock<IMessageContext> WithProducerContext(
        this Mock<IMessageContext> messageContextMock,
        IProducerContext producerContext
    )
    {
        messageContextMock.Setup(x => x.ProducerContext).Returns(producerContext);
        return messageContextMock;
    }

    public static Mock<IMessageContext> WithConsumerContext(
        this Mock<IMessageContext> messageContextMock,
        IConsumerContext consumerContext
    )
    {
        messageContextMock.Setup(x => x.ConsumerContext).Returns(consumerContext);
        return messageContextMock;
    }
}