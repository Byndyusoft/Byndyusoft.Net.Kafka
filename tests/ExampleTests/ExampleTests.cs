using System;
using System.Threading.Tasks;
using Byndyusoft.Example.WebApplication.Dtos;
using Byndyusoft.Example.WebApplication.Producers;
using Byndyusoft.ExampleTests.Fixtures;
using KafkaFlow;
using Moq;
using Xunit;

namespace Byndyusoft.ExampleTests;

public class ExampleTests
{
    private readonly ApiFixture _apiFixture;

    public ExampleTests()
    {
        _apiFixture = new ApiFixture();
    }

    [Fact]
    public async Task ProduceAsync_ProduceMessageToTopic_MessageShouldBeDeliveredToConsumer()
    {
        //Arrange
        const int id = 100;
        const string text = "Hello Kafka!";
        var producedMessageDto = new ExampleMessageDto
        {
            Id = id,
            Text = text
        };
        var producer = _apiFixture.GetService<ExampleProducer>();
        _apiFixture.Tracer.BuildSpan("Test").StartActive(true);
        
        //Act
        await producer.ProduceAsync(producedMessageDto);
        await Task.Delay(2000);

        //Assert
        Mock.Get(_apiFixture.MessageHandler)
            .Verify(mock =>
                mock.Handle(It.IsAny<IMessageContext>(),
                    It.Is<ExampleMessageDto>(dto => dto.Id == id && dto.Text == text)));
    }
}