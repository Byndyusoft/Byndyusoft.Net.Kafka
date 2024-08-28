namespace Byndyusoft.Net.Kafka.Consuming
{
    using System.Threading.Tasks;
    using KafkaFlow;

    public abstract class KafkaMessageHandlerBase<TMessage> : IMessageHandler<TMessage>, IKafkaMessageHandler
    {
        protected abstract Task Handle(TMessage message);

        public Task Handle(IMessageContext context, TMessage message) => Handle(message);
    }
}