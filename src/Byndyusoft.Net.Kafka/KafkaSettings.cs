namespace Byndyusoft.Net.Kafka
{
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
        ///     ClientId or GroupId prefix
        /// </summary>
        public string Prefix { get; set; } = default!;
        
        /// <summary>
        ///     Client name to create unique producer ClientId
        /// </summary>
        public string ClientName { get; set; } = default!;
        
        /// <summary>
        ///     Group name to create unique consumer GroupId
        /// </summary>
        public string GroupName { get; set; } = default!;

        /// <summary>
        ///     True if you want to use Security Information. False if you don`t want to use Security Information.
        /// </summary>
        public bool SecurityInformationEnabled { get; set; }
    }
}