using System;
using System.Collections.Generic;
using System.Text;
using KafkaFlow;
using Newtonsoft.Json;
using OpenTracing;
using OpenTracing.Tag;

namespace Byndyusoft.Net.Kafka.Extensions
{
    public static class SpanExtensions
    {
        public static void SetMessageContext(this ISpan span, IMessageContext messageContext)
        {
            if (messageContext.HasProducerContext())
                SetProducerContext(span, messageContext);

            if (messageContext.HasConsumerContext())
                SetConsumerContext(span, messageContext);
        }

        public static void SetException(this ISpan span, Exception exception)
        {
            span.SetTag(Tags.Error, true);

            span.Log(
                new Dictionary<string, object>(3)
                {
                    { LogFields.Event, Tags.Error.Key },
                    { LogFields.ErrorKind, exception.GetType().Name },
                    { LogFields.ErrorObject, exception }
                }
            );
        }

        private static void SetProducerContext(ISpan span, IMessageContext messageContext)
        {
            var producerContext = messageContext.ProducerContext;
            span.SetTag("kafka.topic", producerContext.Topic);
            
            var log = new Dictionary<string, object>
                      {
                          ["message"] = JsonConvert.SerializeObject(messageContext.Message.Value)
                      };

            var partition = producerContext.Partition;
            if (partition.HasValue)
                log.Add("kafka.partition", partition.Value);

            var offset = producerContext.Offset;
            if (offset.HasValue)
                log.Add("kafka.offset", offset.Value);

            span.Log(log);
        }

        private static void SetConsumerContext(ISpan span, IMessageContext messageContext)
        {
            var consumerContext = messageContext.ConsumerContext;
            span.SetTag("kafka.topic", consumerContext.Topic);

            span.Log(
                new Dictionary<string, object>
                {
                    ["kafka.partition"] = consumerContext.Partition,
                    ["kafka.offset"] = consumerContext.Offset,
                    ["message"] = Encoding.UTF8.GetString((byte[])messageContext.Message.Value)
                }
            );
        }
    }
}