using System;
using System.Threading.Tasks;
using Byndyusoft.Example.WebApplication.Dtos;
using Byndyusoft.Example.WebApplication.Producers;
using Byndyusoft.ExampleTests.Fixtures;
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
        var id = new Random().NextInt64();
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

        //Assert
        var exitCondition = false;
        Mock.Get(_apiFixture.ExampleService)
            .Setup(service => service.DoSomething(It.IsAny<ExampleMessageDto>()))
            .Callback(() => exitCondition = true)
            .Returns(Task.CompletedTask);
        while (exitCondition == false)
        {
            await Task.Delay(100);
        }
        
        Mock.Get(_apiFixture.ExampleService)
            .Verify(service => service.DoSomething(It.Is<ExampleMessageDto>(dto => dto.Id == id && dto.Text == text)), Times.Once);
    }
}