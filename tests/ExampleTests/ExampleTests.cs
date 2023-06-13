using System;
using System.Threading.Tasks;
using Byndyusoft.Example.WebApplication.Dtos;
using Byndyusoft.Example.WebApplication.Producers;
using Byndyusoft.ExampleTests.Fixtures;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace Byndyusoft.ExampleTests;

public class ExampleTests
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly ApiFixture _apiFixture;

    public ExampleTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _apiFixture = new ApiFixture();
    }

    [Fact]
    public async Task ProduceAsync_ProduceMessageToTopic_MessageShouldBeDeliveredToConsumer()
    {
        //Arrange
        var guid = Guid.NewGuid();
        const string text = "Hello Kafka!";
        var producedMessageDto = new ExampleMessageDto
        {
            Guid = guid,
            Text = text
        };
        var producer = _apiFixture.GetService<ExampleProducer>();
        
        //Act
        await producer.ProduceAsync(producedMessageDto);

        //Assert
        var exitCondition = false;
        Mock.Get(_apiFixture.ExampleService)
            .Setup(service => service.DoSomething(It.IsAny<ExampleMessageDto>()))
            .Callback((ExampleMessageDto messageDto) =>
            {
                _testOutputHelper.WriteLine($"Arrived message with Id: {messageDto.Guid} and text {messageDto.Text}");
                exitCondition = true;
            })
            .Returns(Task.CompletedTask);
        while (exitCondition == false)
        {
            await Task.Delay(100);
        }
        
        // Без этого ожидания, оффсет не будет увеличен (сервис не успевает его поднять)
        await Task.Delay(5000);

        Mock.Get(_apiFixture.ExampleService)
            .Verify(service => service.DoSomething(It.Is<ExampleMessageDto>(dto => dto.Guid == guid && dto.Text == text)), Times.Once);
    }
}