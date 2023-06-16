﻿using Byndyusoft.Example.Domain.Dtos;
using Byndyusoft.Net.Kafka.Abstractions;
using KafkaFlow.Producers;

namespace Byndyusoft.Example.Domain.Producers
{
    public sealed class ExampleProducer : KafkaProducerBase<ExampleMessageDto>
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