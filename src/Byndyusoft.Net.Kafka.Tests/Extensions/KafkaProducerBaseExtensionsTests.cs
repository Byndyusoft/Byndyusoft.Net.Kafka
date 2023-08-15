namespace Byndyusoft.Net.Kafka.Tests.Extensions;

using Common.Generators;
using FluentAssertions;
using Kafka.Extensions;
using Xunit;

public class KafkaProducerBaseExtensionsTests
{
    [Fact]
    public void BuildingProducerClientId()
    {
        //Arrange
        const string prefix = "Byndyusoft.SomeProject.Api";
        var producerMock = KafkaProducerMockGenerator
            .Empty()
            .WithTopic("project.entity.creation")
            .Object;

        //Act
        var consumersGroupId = producerMock.BuildClientId(prefix);

        //Assert
        consumersGroupId.Should().Be("byndyusoft.some_project_api.project_entity_creation");
    }
}