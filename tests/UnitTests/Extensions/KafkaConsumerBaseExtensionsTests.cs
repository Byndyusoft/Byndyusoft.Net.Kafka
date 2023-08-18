namespace Byndyusoft.Net.Kafka.Tests.Extensions;

using Byndyusoft.Net.Kafka.Extensions;
using Common.Generators;
using FluentAssertions;
using Xunit;

public class KafkaConsumerBaseExtensionsTests
{
    [Fact]
    public void BuildingConsumersGroupId()
    {
        //Arrange
        const string prefix = "Byndyusoft.SomeProject.Api";
        var consumerMock = KafkaConsumerMockGenerator
            .Empty()
            .WithTopic("project.entity.creation")
            .Object;

        //Act
        var consumersGroupId = consumerMock.BuildConsumersGroupId(prefix);

        //Assert
        consumersGroupId.Should().Be("byndyusoft.some_project_api.project_entity_creation");
    }
}