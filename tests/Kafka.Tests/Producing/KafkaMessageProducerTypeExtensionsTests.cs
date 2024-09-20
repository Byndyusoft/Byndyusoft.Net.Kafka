namespace Byndyusoft.Net.Kafka.Tests.Producing
{
    using Abstractions.Producing;
    using FluentAssertions;
    using Kafka.Producing;
    using Xunit;

    public class KafkaMessageProducerTypeExtensionsTests
    {
        [Fact]
        public void ShouldGetKafkaTopic()
        {
            // When
            var actualTopicName = typeof(EntityCreationEventMessageDefaultProducer).GetTopic();

            // Then
            actualTopicName.Should().Be("composer_assistant.entity.creation");
        }

        [Fact]
        public void ShouldBuildKafkaTopic()
        {
            // Given
            const string solutionName = "MusicalityLabs.ComposerAssistant.Storage.Api";

            // When
            var actualTopicName = typeof(EntityCreationEventMessageDefaultProducer).BuildClientId(solutionName);

            // Then
            actualTopicName.Should().Be("composer_assistant.storage_api.entity_creation");
        }

        [Fact]
        public void ShouldGetDefaultProducingProfileName()
        {
            // When
            var actualProducingProfileName = typeof(EntityCreationEventMessageDefaultProducer).GetProducingProfileName();

            // Then
            actualProducingProfileName.Should().Be("byndyusoft.net.kafka.tests.producing.entity_creation_event_message_default_producer");
        }

        [Fact]
        public void ShouldGetCustomProducingProfileName()
        {
            // When
            var actualProducingProfileName = typeof(EntityCreationEventMessageCustomProducer).GetProducingProfileName();

            // Then
            actualProducingProfileName.Should().Be("entity_creation_event_message_custom_producer");
        }
    }
}