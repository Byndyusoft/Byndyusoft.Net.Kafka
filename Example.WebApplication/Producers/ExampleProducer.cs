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
        public override string KeyGenerator(ExampleMessageDto message)
        {
            return message.Id.ToString();
        }
    }

    public class ExampleMessageDto
    {
        public string Text { get; set; }
        
        public long Id { get; set;}
    }
}