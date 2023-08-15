namespace Byndyusoft.Net.Kafka.Tests.Extensions;

using FluentAssertions;
using Kafka.Extensions;
using KafkaFlow;
using Moq;
using Newtonsoft.Json;
using OpenTracing;
using OpenTracing.Mock;
using OpenTracing.Tag;
using System;
using Common.Generators;
using Xunit;
using System.Text;

public class SpanExtensionsTests
{
    private readonly object _messageContent;
    private readonly Message _message;

    public SpanExtensionsTests()
    {
        _messageContent = new { Id = 1, Content = "message_content" };
        _message = new Message(
            "message_key",
            Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(_messageContent))
        );
    }

    [Fact]
    public void SpanShouldBeTaggedAsErroredAndIncludeExceptionInfo()
    {
        //Act
        var tracer = new MockTracer();
        var span = tracer
            .BuildSpan(nameof(SpanShouldBeTaggedAsErroredAndIncludeExceptionInfo))
            .Start();
        var exception = new Exception("some error");

        //Arrange
        span.SetException(exception);
        span.Finish();

        //Assert
        var finishedSpans = tracer.FinishedSpans();
        finishedSpans.Should().HaveCount(1);

        var finishedSpan = finishedSpans.Single();
        finishedSpan.OperationName.Should().Be(nameof(SpanShouldBeTaggedAsErroredAndIncludeExceptionInfo));

        var tags = finishedSpan.Tags;
        tags[Tags.Error.Key].Should().Be(true);

        var log = finishedSpan.LogEntries.Single();
        log.Fields[LogFields.Event].Should().Be(Tags.Error.Key);
        log.Fields[LogFields.ErrorKind].Should().Be(exception.GetType().Name);
        log.Fields[LogFields.ErrorObject].Should().Be(exception);
    }

    [Fact]
    public void SpanShouldSetConsumerMessageContext()
    {
        //Act
        var tracer = new MockTracer();
        var span = tracer
            .BuildSpan(nameof(SpanShouldSetConsumerMessageContext))
            .Start();

        var producerContextMock = ProducerContextMockGenerator
            .TestProducerContextMock()
            .Object;

        var messageContextMock = new Mock<IMessageContext>()
            .WithProducerContext(producerContextMock)
            .WithMessage(_message.Key, _message.Value)
            .Object;

        //Arrange
        span.SetMessageContext(messageContextMock);
        span.Finish();

        //Assert
        var finishedSpans = tracer.FinishedSpans();
        finishedSpans.Should().HaveCount(1);

        var finishedSpan = finishedSpans.Single();
        finishedSpan.OperationName.Should().Be(nameof(SpanShouldSetConsumerMessageContext));

        var tags = finishedSpan.Tags;
        tags["kafka.topic"].Should().Be(producerContextMock.Topic);

        var log = finishedSpan.LogEntries.Single();
        log.Fields["message"].Should().Be(JsonConvert.SerializeObject(_message.Value));
        log.Fields["kafka.partition"].Should().Be(producerContextMock.Partition);
        log.Fields["kafka.offset"].Should().Be(producerContextMock.Offset);
    }


    [Fact]
    public void SpanShouldSetProducerMessageContext()
    {
        //Act
        var tracer = new MockTracer();
        var span = tracer
            .BuildSpan(nameof(SpanShouldSetConsumerMessageContext))
            .Start();

        var consumerContextMock = ConsumerContextMockGenerator
            .TestConsumerContextMock()
            .Object;

        var messageContextMock = new Mock<IMessageContext>()
            .WithConsumerContext(consumerContextMock)
            .WithMessage(_message.Key, _message.Value)
            .Object;

        //Arrange
        span.SetMessageContext(messageContextMock);
        span.Finish();

        //Assert
        var finishedSpans = tracer.FinishedSpans();
        finishedSpans.Should().HaveCount(1);

        var finishedSpan = finishedSpans.Single();
        finishedSpan.OperationName.Should().Be(nameof(SpanShouldSetConsumerMessageContext));

        var tags = finishedSpan.Tags;
        tags["kafka.topic"].Should().Be(consumerContextMock.Topic);

        var log = finishedSpan.LogEntries.Single();
        log.Fields["message"].Should().Be(JsonConvert.SerializeObject(_messageContent));
        log.Fields["kafka.partition"].Should().Be(consumerContextMock.Partition);
        log.Fields["kafka.offset"].Should().Be(consumerContextMock.Offset);
    }
}