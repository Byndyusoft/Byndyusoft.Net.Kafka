namespace Byndyusoft.Net.Kafka.Configuration
{
    using KafkaFlow.OpenTelemetry;
    using OpenTelemetry.Trace;

    public static class TracerProviderBuilderExtensions
    {
        public static TracerProviderBuilder AddKafkaInstrumentation(this TracerProviderBuilder builder)
            => builder.AddSource(KafkaFlowInstrumentation.ActivitySourceName);
    }
}