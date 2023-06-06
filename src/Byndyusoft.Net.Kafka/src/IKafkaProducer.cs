using System.Threading.Tasks;

namespace Byndyusoft.Net.Kafka
{
    public interface IKafkaProducer
    {
        public string Title { get; }
        public string Topic { get; }
    }

    public interface IKafkaProducer<in T> : IKafkaProducer
    {
        public string KeyGenerator(T message);
        public Task ProduceAsync(T message);
    }
}