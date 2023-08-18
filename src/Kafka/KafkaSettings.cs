namespace Byndyusoft.Net.Kafka
{
    using KafkaFlow.Configuration;

    /// <summary>
    ///     Settings to connection with Kafka
    /// </summary>
    public class KafkaSettings
    {
        /// <summary>
        ///     Available hosts
        /// </summary>
        public string[] Hosts { get; set; } = default!;

        /// <summary>
        ///     Represent the Kafka security information
        /// </summary>
        public SecurityInformation? SecurityInformation { get; set; }
    }
}