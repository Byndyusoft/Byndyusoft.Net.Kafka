namespace Byndyusoft.Net.Kafka
{
    /// <summary>
    /// Settings to connection with Kafka
    /// </summary>
    public class KafkaSettings
    {
        /// <summary>
        /// Available hosts
        /// </summary>
        public string[] Hosts { get; set; } = default!;

        /// <summary>
        /// Connection user name
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Connection password
        /// </summary>
        public string Password { get; set; }
    }
}