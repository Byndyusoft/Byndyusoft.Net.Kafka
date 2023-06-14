using System;
using System.Threading.Tasks;
using Byndyusoft.Net.Kafka.Extensions;
using KafkaFlow;
using OpenTracing;
using OpenTracing.Tag;

namespace Byndyusoft.Net.Kafka.Middlewares
{
    internal class PublishMessageTracingMiddleware : IMessageMiddleware
    {
        private readonly ITracer _tracer;

        public PublishMessageTracingMiddleware(ITracer tracer)
        {
            _tracer = tracer ?? throw new ArgumentNullException(nameof(tracer));
        }

        public async Task Invoke(IMessageContext context, MiddlewareDelegate next)
        {
            var span = _tracer.BuildSpan("publish")
                .WithTag(Tags.SpanKind.Key, Tags.SpanKindProducer)
                .AsChildOf(_tracer.ActiveSpan)
                .Start();

            span.SetMessageContext(context);
            _tracer.InjectMessageContextHeaders(span, context);

            await next(context);

            span.Finish();
        }
    }
}