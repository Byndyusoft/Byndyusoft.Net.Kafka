using KafkaFlow.Configuration;

namespace Byndyusoft.Net.Kafka
{
    /// <summary>
    ///     TODO
    /// </summary>
    public class KafkaSecurityInformationSettings
    {
        /// <summary>
        ///     TODO
        /// </summary>
        public string Password { get; set; } = default!;
        
        /// <summary>
        ///     TODO
        /// </summary>
        public string Username { get; set; } = default!;
        
        /// <summary>
        ///     TODO
        /// </summary>
        public SaslMechanism SaslMechanism { get; set; }
        
        /// <summary>
        ///     TODO
        /// </summary>
        public SecurityProtocol SecurityProtocol { get; set; }
    }
}