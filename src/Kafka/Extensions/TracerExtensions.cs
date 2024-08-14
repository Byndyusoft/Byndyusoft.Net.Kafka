namespace Byndyusoft.Net.Kafka.Extensions
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using KafkaFlow;
    using OpenTracing;
    using OpenTracing.Propagation;

    public static class TracerExtensions
    {
        public static void InjectMessageContextHeaders(
            this ITracer tracer,
            ISpan span,
            IMessageContext messageContext
        )
        {
            var carriers = new Dictionary<string, string>();
            tracer.Inject(span.Context, BuiltinFormats.HttpHeaders, new TextMapInjectAdapter(carriers));
            foreach (var carrier in carriers)
                messageContext.Headers.Add(carrier.Key, Encoding.UTF8.GetBytes(carrier.Value));
        }

        public static ISpanContext CreateSpanContextFromMessageContext(
            this ITracer tracer,
            IMessageContext messageContext
        )
        {
            var contextHeaders = messageContext.Headers.ToDictionary(x => x.Key, x => Encoding.UTF8.GetString(x.Value));

            return tracer.Extract(BuiltinFormats.HttpHeaders, new TextMapExtractAdapter(contextHeaders));
        }
    }
}