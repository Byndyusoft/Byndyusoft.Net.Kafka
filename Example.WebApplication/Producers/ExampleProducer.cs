using Byndyusoft.Example.WebApplication.Dtos;
using Byndyusoft.Net.Kafka;
using KafkaFlow.Producers;

namespace Byndyusoft.Example.WebApplication.Producers
{
    public class ExampleProducer : KafkaProducerBase<ExampleMessageDto>
    {
        public ExampleProducer(IProducerAccessor producers) : base(producers, "Title")
        {
        }

        public override string Topic  => "topic";
        public override string ClientName => "example_client";

        public override string KeyGenerator(ExampleMessageDto message)
        {
            return message.Guid.ToString();
        }
    }
}