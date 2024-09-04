namespace Byndyusoft.Net.Kafka.Tests.Consuming
{
    using Abstractions.Consuming;
    using FluentAssertions;
    using Kafka.Consuming;
    using Xunit;

    public class KafkaMessageHandlerTypeExtensionsTests
    {
        [Fact]
        public void ShouldGetKafkaTopic()
        {
            // When
            var actualTopicName = typeof(EntityCreationMessageHandler).GetTopic();

            // Then
            actualTopicName.Should().Be("composer_assistant.entity.creation");
        }

        [Fact]
        public void ShouldBuildKafkaConsumersGroupId()
        {
            // Given
            const string solutionName = "MusicalityLabs.ComposerAssistant.Storage.Api";

            // When
            var actualConsumerGroupId = typeof(EntityCreationMessageHandler).BuildConsumersGroupId(solutionName);

            // Then
            actualConsumerGroupId.Should().Be("composer_assistant.storage_api.entity_creation");
        }
    }
}