using KafkaFlow.Configuration;

namespace Byndyusoft.Net.Kafka
{
    /// <summary>
    ///     Settings to enable Kafka Security Information
    /// </summary>
    public class KafkaSecurityInformationSettings
    {
        /// <summary>
        ///     Username
        /// </summary>
        public string Username { get; set; } = default!;

        /// <summary>
        ///     Password
        /// </summary>
        public string Password { get; set; } = default!;

        /// <summary>
        ///     SASL Mechanism
        /// </summary>
        public SaslMechanism SaslMechanism { get; set; }
        
        /// <summary>
        ///     Security Protocol
        /// </summary>
        public SecurityProtocol SecurityProtocol { get; set; }
    }
}