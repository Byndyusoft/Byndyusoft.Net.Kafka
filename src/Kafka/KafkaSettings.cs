namespace Byndyusoft.Net.Kafka
{
    using KafkaFlow.Configuration;

    /// <summary>
    ///     Settings to connection with Kafka
    /// </summary>
    public class KafkaSettings
    {
        public KafkaSettings()
        {
            ProducerSettings = new KafkaProducerSettings();
        }

        /// <summary>
        ///     Available hosts
        /// </summary>
        public string[] Hosts { get; set; } = default!;

        /// <summary>
        ///     Represent the Kafka producer settings
        /// </summary>
        public KafkaProducerSettings ProducerSettings { get; set; }

        /// <summary>
        ///     Represent the Kafka security information
        /// </summary>
        public SecurityInformation? SecurityInformation { get; set; }
    }
}