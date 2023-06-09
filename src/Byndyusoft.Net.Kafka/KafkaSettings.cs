namespace Byndyusoft.Net.Kafka
{
    /// <summary>
    ///     TODO
    /// </summary>
    public class KafkaSettings
    {
        /// <summary>
        ///     TODO
        /// </summary>
        public string[] Hosts { get; set; } = default!;
        
        /// <summary>
        ///     TODO
        /// </summary>
        public string Prefix { get; set; } = default!;
        
        /// <summary>
        ///     TODO
        /// </summary>
        public string ClientName { get; set; } = default!;
        
        /// <summary>
        ///     TODO
        /// </summary>
        public string GroupName { get; set; } = default!;

        /// <summary>
        ///     TODO
        /// </summary>
        public bool SecurityInformationEnabled { get; set; }
    }
}