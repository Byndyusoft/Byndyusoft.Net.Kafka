namespace Byndyusoft.Net.Kafka.Abstractions.Producing
{
    using System.Threading.Tasks;

    /// <summary>
    /// Kafka low level message sender contract
    /// </summary>
    public interface IKafkaMessageSender
    {
        /// <summary>
        /// Sends message <paramref name="message"/> with key <paramref name="messageKey"/> to Kafka by using producing profile <paramref name="producingProfileName"/>
        /// </summary>
        /// <param name="producingProfileName">Producing profile name</param>
        /// <param name="messageKey">Message key</param>
        /// <param name="message">Message</param>
        Task SendAsync<TMessage>(string producingProfileName, string messageKey, TMessage message);
    }
}